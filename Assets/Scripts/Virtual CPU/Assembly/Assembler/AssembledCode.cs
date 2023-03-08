namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class AssembledCode
    {
        public byte[] Code { get; }
        public bool[] Mask { get; }
        
        public ushort ResetVector { get; }
        public ushort IrqVector { get; }
        public ushort NmiVector { get; }

        private const ushort ResetVectorAddress = 0xFFFC;
        private const ushort NmiVectorAddress = 0xFFFA;
        private const ushort IrqVectorAddress = 0xFFFE;

        public AssembledCode(byte[] code, bool[] mask, Vectors vectors)
        {
            Code = code;
            Mask = mask;
            ResetVector = vectors.Reset;
            IrqVector = vectors.Irq;
            NmiVector = vectors.Nmi;
        }

        private void WriteVector(ushort address, ushort value)
        {
            if (Code is null || Mask is null) return;
            
            Code[address] = (byte)(value & 0xFF);
            Code[address + 1] = (byte)((value >> 8) & 0xFF);
            Mask[address] = Mask[address + 1] = true;
        }

        public void WriteResetVector() => WriteVector(ResetVectorAddress, ResetVector);
        public void WriteIrqVector() => WriteVector(IrqVectorAddress, IrqVector);
        public void WriteNmiVector() => WriteVector(NmiVectorAddress, NmiVector);
    }
}