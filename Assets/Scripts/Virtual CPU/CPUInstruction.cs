using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU
{
    public abstract class CPUInstruction
    {
        public byte[] args;

        public abstract string Mnemonic { get; }
        public abstract Dictionary<AddressingMode, CPUInstructionMetadata> Metadata { get; }

        public abstract void Execute(CPU cpu, AddressingMode addressingMode);

        public void Setup(params byte[] args)
        {
            this.args = args;
        }
    }
}
