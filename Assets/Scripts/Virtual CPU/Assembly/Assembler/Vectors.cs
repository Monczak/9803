namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Vectors
    {
        private ushort reset = 0;
        private ushort irq = 0;
        private ushort nmi = 0;

        public ushort Reset
        {
            get => reset;
            set 
            { 
                reset = value;
                ResetSet = true;
            }
        }

        public ushort Irq
        {
            get => irq;
            set
            {
                irq = value;
                IrqSet = true;
            }
        }

        public ushort Nmi
        {
            get => nmi;
            set
            {
                nmi = value;
                NmiSet = true;
            }
        }

        public bool ResetSet { get; private set; } = false;
        public bool IrqSet { get; private set; } = false;
        public bool NmiSet { get; private set; } = false;

        public static readonly ushort ResetAddress = 0xFFFC;
        public static readonly ushort IrqAddress = 0xFFFE;
        public static readonly ushort NmiAddress = 0xFFFA;
    }
}