using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using NineEightOhThree.VirtualCPU.Interfacing;
using NineEightOhThree.VirtualCPU.Utilities;
using UnityEngine;

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

        public bool showDebugInfo;

        private CPUInstruction currentInstruction;
        private AddressingMode addressingMode;

        public BindableManager BindableManager { get; private set; }

        private void SetStatusRegisterBit(byte bit, bool set)
        {
            BitUtils.SetBit(ref statusRegister, bit, set);
        }

        private void Awake()
        {
            Memory = GetComponent<Memory>();
            BindableManager = GetComponent<BindableManager>();

            Instance = this;

            InitProcessor();
        }

        // Start is called before the first frame update
        void Start()
        {
            /*// var program = Assembler.Assemble("lda #$00\nldx #$10\nclc\nadc #$02\ndex\nbne $fa\njmp $0000");
            var program = Assembler.Assemble("lda #$01\nsta $0200\nsta $0201\nldx #$00\nlda $0200,x\nclc\nadc $0201,x\nsta $0202,x\ninx\ncpx #$fe\nbne $f1\njmp $0008");
            // var program = Assembler.Assemble("inc $0300\ninc $0301\njmp $0000");
            for (int i = 0x00; i < program.Count; i++)
                Memory.Write((ushort)i, program[i]);*/

            string code = @"ldx #0
loop: lda $0300,x
sta $0400,x
inx
beq loop";
            Assembler.Assemble(code);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            Cycle();

            BindableManager.Synchronize();
        }

        public void InitProcessor()
        {
            StackPointer = StackTopPointer;
        }

        public void PushStack(byte b)
        {
            Memory.Write(StackPointer--, b);
            if (StackPointer == (ushort)(StackTopPointer - stackSize - 1))
                throw new StackOverflowException();
        }

        public byte PullStack()
        {
            byte b = Memory.Read(StackPointer++);
            if (StackPointer == (ushort)(StackTopPointer + 1))
                throw new StackUnderflowException();
            return b;
        }

        public void Cycle()
        {
            preCycleProgramCounter = ProgramCounter;

            Fetch();
            Execute();
            if (showDebugInfo) PrintStatus();
        }

        private void Fetch()
        {
            try
            {
                CPUInstructionMetadata metadata;
                (currentInstruction, addressingMode, metadata) = CPUInstructionRegistry.GetInstruction(Memory.Read(ProgramCounter));
                byte[] args = Memory.ReadBlock((ushort)(ProgramCounter + 1), metadata.ArgumentCount);
                currentInstruction.Setup(args);

                ProgramCounter += (ushort)(1 + args.Length);
            }
            catch (UnknownOpcodeException e)
            {
                Debug.LogError($"Unknown opcode {e.Opcode} at {ProgramCounter:X4}");
            }
        }

        private void Execute()
        {
            currentInstruction?.Execute(this, addressingMode);
        }

        private void PrintStatus()
        {
            Debug.Log($"PC: {preCycleProgramCounter:X4} | {currentInstruction.Mnemonic} {string.Join(" ", currentInstruction.args)} | A: {RegisterA} X: {RegisterX} Y: {RegisterY}");
        }
    }
}

