using NineEightOhThree.VirtualCPU.Utilities;
using UnityEngine;

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

        public byte ProgramCounter { get; protected internal set; }
        private byte preCycleProgramCounter;

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
            for (byte i = 0x00; i < 0xFF; i++)
                Memory.Write(i, 0xEA);  // NOP

            Memory.Write(0x00, 0xA9);   // LDA 1
            Memory.Write(0x01, 0x01);

            Memory.Write(0x02, 0xA9);   // LDA 2
            Memory.Write(0x03, 0x02);

            Memory.Write(0x04, 0xA9);   // LDA 3
            Memory.Write(0x05, 0x03);

            Memory.Write(0x06, 0x18);   // CLC

            Memory.Write(0x07, 0x69);   // ADC 1
            Memory.Write(0x08, 0x01);

            Memory.Write(0x0a, 0x4C);   // JMP 6
            Memory.Write(0x0b, 0x06);
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
                byte[] args = Memory.ReadBlock((byte)(ProgramCounter + 1), metadata.ArgumentCount);
                currentInstruction.Setup(args);

                ProgramCounter += (byte)(1 + args.Length);
            }
            catch (UnknownOpcodeException e)
            {
                Debug.LogError($"Unknown opcode {e.Opcode} at {ProgramCounter}");
            }
        }

        private void Execute()
        {
            currentInstruction.Execute(this, addressingMode);
        }

        private void PrintStatus()
        {
            Debug.Log($"PC: {preCycleProgramCounter} | {currentInstruction.Mnemonic} {string.Join(" ", currentInstruction.args)} | A: {RegisterA} X: {RegisterX} Y: {RegisterY}");
        }
    }
}

