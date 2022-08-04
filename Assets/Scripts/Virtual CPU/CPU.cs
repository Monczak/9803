using NineEightOhThree.VirtualCPU.Utilities;
using UnityEngine;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;

namespace NineEightOhThree.VirtualCPU
{
    [RequireComponent(typeof(Memory))]
    public class CPU : MonoBehaviour
    {
        [HideInInspector]
        public Memory Memory { get; private set; }

        public byte RegisterA { get; protected internal set; }
        public byte RegisterX { get; protected internal set; }
        public byte RegisterY { get; protected internal set; }

        private byte statusRegister;
        public byte StatusRegister
        {
            get => statusRegister;
            protected internal set
            {
                statusRegister = value;
            }
        }
        public bool NegativeFlag
        {
            get => (StatusRegister & 0b10000000) != 0;
            set { SetStatusRegisterBit(7, value); }
        }
        public bool OverflowFlag
        {
            get => (StatusRegister & 0b01000000) != 0;
            set { SetStatusRegisterBit(6, value); }
        }
        public bool ZeroFlag
        {
            get => (StatusRegister & 0b00000010) != 0;
            set { SetStatusRegisterBit(1, value); }
        }
        public bool CarryFlag
        {
            get => (StatusRegister & 0b00000001) != 0;
            set { SetStatusRegisterBit(0, value); }
        }

        public ushort ProgramCounter { get; protected internal set; }
        private ushort preCycleProgramCounter;

        public int stackSize = 256;
        public byte[] Stack { get; protected internal set; }
        public byte StackPointer { get; protected internal set; }


        private CPUInstruction currentInstruction;
        private AddressingMode addressingMode;

        private void SetStatusRegisterBit(byte bit, bool set)
        {
            BitUtils.SetBit(ref statusRegister, bit, set);
        }

        private void Awake()
        {
            Memory = GetComponent<Memory>();

            InitProcessor();
        }

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0x00; i < Memory.size; i++)
                Memory.Write((byte)i, 0xEA);  // NOP

            var program = Assembler.Assemble("lda #$00\nldx #$10\nclc\nadc #$02\ndex\ncpx #$00\nbne $f8\njmp $0000");
            for (int i = 0x00; i < program.Count; i++)
                Memory.Write((byte)i, program[i]);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            Cycle();
        }

        public void InitProcessor()
        {
            Stack = new byte[stackSize];
            StackPointer = 0xFF;
        }

        public void PushStack(byte b)
        {
            Stack[StackPointer--] = b;
            StackPointer %= (byte)stackSize;
            if (StackPointer == stackSize - 1)
                throw new StackOverflowException();
        }

        public byte PullStack()
        {
            byte b = Stack[StackPointer++];
            StackPointer %= (byte)stackSize;
            if (StackPointer == 0x00)
                throw new StackUnderflowException();
            return b;
        }

        public void Cycle()
        {
            preCycleProgramCounter = ProgramCounter;

            Fetch();
            Execute();
            PrintStatus();
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
            currentInstruction.Execute(this, addressingMode);
        }

        private void PrintStatus()
        {
            Debug.Log($"PC: {preCycleProgramCounter:X4} | {currentInstruction.Mnemonic} {string.Join(" ", currentInstruction.args)} | A: {RegisterA} X: {RegisterX} Y: {RegisterY}");
        }
    }
}

