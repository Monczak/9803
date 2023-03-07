using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NineEightOhThree.Managers;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
using NineEightOhThree.VirtualCPU.Interfacing;
using NineEightOhThree.VirtualCPU.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NineEightOhThree.VirtualCPU
{
    [RequireComponent(typeof(Memory)), RequireComponent(typeof(BindableManager))]
    public class CPU : MonoBehaviour
    {
        public static CPU Instance { get; private set; }
        
        public Memory Memory { get; private set; }

        public byte RegisterA { get; protected internal set; }
        public byte RegisterX { get; protected internal set; }
        public byte RegisterY { get; protected internal set; }

        private byte statusRegister;
        public byte StatusRegister
        {
            get => statusRegister;
            protected internal set => statusRegister = value;
        }

        private bool interruptRequest;
        private bool nonMaskableInterrupt;
        
        public bool NegativeFlag
        {
            get => (StatusRegister & 0b10000000) != 0;
            set => SetStatusRegisterBit(7, value);
        }
        public bool OverflowFlag
        {
            get => (StatusRegister & 0b01000000) != 0;
            set => SetStatusRegisterBit(6, value);
        }

        public bool InterruptDisableFlag
        {
            get => (StatusRegister & 0b00000100) != 0;
            set => SetStatusRegisterBit(2, value);
        }
        public bool ZeroFlag
        {
            get => (StatusRegister & 0b00000010) != 0;
            set => SetStatusRegisterBit(1, value);
        }
        public bool CarryFlag
        {
            get => (StatusRegister & 0b00000001) != 0;
            set => SetStatusRegisterBit(0, value);
        }

        public ushort ProgramCounter { get; protected internal set; }
        private ushort preCycleProgramCounter;

        public ushort StackTopPointer { get; protected internal set; } = 0x01FF;

        public int stackSize = 256;
        public ushort StackPointer { get; protected internal set; }
        
        private ushort ResetVector => (ushort)BitUtils.FromLittleEndian<ushort>(Memory.ReadBlock(0xFFFC, 2));
        private ushort IrqVector => (ushort)BitUtils.FromLittleEndian<ushort>(Memory.ReadBlock(0xFFFE, 2));
        private ushort NmiVector => (ushort)BitUtils.FromLittleEndian<ushort>(Memory.ReadBlock(0xFFFA, 2));
        
        
        public bool showDebugInfo;

        private readonly object lockObj = new();

        private CPUInstruction currentInstruction;
        private AddressingMode addressingMode;

        public BindableManager BindableManager { get; private set; }

        private Thread cpuThread;
        public bool running;
        public double cyclesPerSecond;

        private double[] executionTimes;
        private int executionTimeIndex;

        private void SetStatusRegisterBit(byte bit, bool set)
        {
            BitUtils.SetBit(ref statusRegister, bit, set);
        }

        private void Awake()
        {
            Memory = GetComponent<Memory>();
            BindableManager = GetComponent<BindableManager>();

            Instance = this;

            Memory.Setup();
            InitProcessor();

            executionTimes = new double[500];
            executionTimeIndex = 0;
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCPUThread();
        }
        
        // Update is called once per frame
        void Update()
        {
            double currentSpeed = 1 / (executionTimes.Average() / 1000);
            // Logger.Log($"Speed: {currentSpeed:F2} ({currentSpeed / cyclesPerSecond * 100:F2}%)");
            
            BindableManager.Synchronize();
        }

        private void StartCPUThread()
        {
            cpuThread = new Thread(StartCPU);
            cpuThread.Start();
        }

        private void StartCPU()
        {
            Reset();
            while (true)
            {
                if (running)
                {
                    CycleAndWait();
                }
            }
        }

        public void Reset()
        {
            InitProcessor();
            ProgramCounter = ResetVector;
            StatusRegister = 0b00000000;
            RegisterA = 0;
            RegisterX = 0;
            RegisterY = 0;
        }

        private void CycleAndWait()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Cycle();
                    
            double delayMs = 1 / cyclesPerSecond * 1000;
            while (stopwatch.Elapsed.TotalMilliseconds < delayMs)
            {
                // Spin
            }

            double time = stopwatch.Elapsed.TotalMilliseconds;
            executionTimes[executionTimeIndex] = time;
            executionTimeIndex = (executionTimeIndex + 1) % executionTimes.Length;
        }

        public void SetIrq()
        {
            interruptRequest = true;
        }

        public void SetNmi()
        {
            nonMaskableInterrupt = true;
        }

        public void InitProcessor()
        {
            StackPointer = StackTopPointer;
        }

        public void WriteCode(byte[] code, bool[] codeMask)
        {
            ushort byteCount = 0;

            lock (lockObj)
            {
                for (int i = 0; i < code.Length; i++)
                {
                    if (codeMask[i])
                    {
                        Memory.Write((ushort)i, code[i]);
                        byteCount++;
                    }
                }
            }
            Logger.Log($"{byteCount} bytes written");

            ProgramCounter = ResetVector;
        }

        
        public void PushStack(byte b)
        {
            Memory.Write(StackPointer--, b);
            if (StackPointer == (ushort)(StackTopPointer - stackSize - 1))
                throw new StackOverflowException();
        }

        public byte PullStack()
        {
            byte b = Memory.Read(++StackPointer);
            if (StackPointer == (ushort)(StackTopPointer + 1))
                throw new StackUnderflowException();
            return b;
        }

        public void Cycle()
        {
            preCycleProgramCounter = ProgramCounter;

            if (nonMaskableInterrupt || (interruptRequest && !InterruptDisableFlag))
            {
                PushStack((byte)((ProgramCounter >> 8) & 0xFF));
                PushStack((byte)(ProgramCounter & 0xFF));
                
                PushStack(StatusRegister);
                
                InterruptDisableFlag = true;
                if (nonMaskableInterrupt)
                    ProgramCounter = NmiVector;
                else if (interruptRequest) 
                    ProgramCounter = IrqVector;

                interruptRequest = false;
                nonMaskableInterrupt = false;
            }

            Fetch();
            Execute();
            if (showDebugInfo) PrintStatus();
        }

        private void Fetch()
        {
            byte opcode = Memory.Read(ProgramCounter);
            if (!CPUInstructionRegistry.TryGetInstruction(opcode, out var data) || data is null)
            {
                Logger.LogError($"Unknown opcode {opcode} at {ProgramCounter:X4}");
                return;
            }

            currentInstruction = data.Value.instruction;
            addressingMode = data.Value.addressingMode;
                
            byte[] args = Memory.ReadBlock((ushort)(ProgramCounter + 1), data.Value.metadata.ArgumentCount);
            currentInstruction.Setup(args);

            ProgramCounter += (ushort)(1 + args.Length);
        }

        private void Execute()
        {
            currentInstruction?.Execute(this, addressingMode);
        }

        private void PrintStatus()
        {
            Logger.Log($"PC: {preCycleProgramCounter:X4} | {currentInstruction.Mnemonic} {string.Join(" ", currentInstruction.args)} | A: {RegisterA} X: {RegisterX} Y: {RegisterY}");
        }
    }
}

