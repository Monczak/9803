namespace NineEightOhThree.VirtualCPU
{
    public struct CPUInstructionMetadata
    {
        public byte Opcode { get; init; }
        public byte ArgumentCount { get; init; }

        public CPUInstructionMetadata(byte opcode, byte argumentCount)
        {
            Opcode = opcode;
            ArgumentCount = argumentCount;
        }
    }
}
