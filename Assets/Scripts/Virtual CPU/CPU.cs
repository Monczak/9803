using System;
using NineEightOhThree.Managers;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
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

        private readonly object lockObj = new();

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
            
            string code = @"
nums: .byte $ff $00 $de $ad $be $ef
.org $8000
ldx #0
[]          ; Error: unexpected character
blah        ; Error: unknown instruction
loop: lda $0300,x
asl a
sta $0400,  ; Error: unfinished statement
inx a       ; Error: unsupported addressing
cmp ($03),y
inx
bne end
beq loop
end: jmp loop";
            AssemblerInterface.ScheduleAssembly(code, WriteCode);
        }

        public void WriteCode(Assembler.AssemblerResult result)
        {
            ushort byteCount = 0;

            lock (lockObj)
            {
                for (int i = 0; i < result.Code.Length; i++)
                {
                    if (result.CodeMask[i])
                    {
                        Memory.Write((ushort)i, result.Code[i]);
                        byteCount++;
                    }
                }
            }
            Debug.Log($"{byteCount} bytes written");
            

            ProgramCounter = 0; // TODO: Read from reset vector
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
            byte opcode = Memory.Read(ProgramCounter);
            if (!CPUInstructionRegistry.TryGetInstruction(opcode, out var data) || data is null)
            {
                Debug.LogError($"Unknown opcode {opcode} at {ProgramCounter:X4}");
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
            Debug.Log($"PC: {preCycleProgramCounter:X4} | {currentInstruction.Mnemonic} {string.Join(" ", currentInstruction.args)} | A: {RegisterA} X: {RegisterX} Y: {RegisterY}");
        }
    }
}

