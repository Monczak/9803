namespace NineEightOhThree.VirtualCPU
{
    public abstract class CPUInstruction
    {
        public abstract string Mnemonic { get; }
        public abstract byte Opcode { get; }
        public abstract int ArgumentCount { get; }

        public abstract void Execute(params byte[] @params);
    }
}
