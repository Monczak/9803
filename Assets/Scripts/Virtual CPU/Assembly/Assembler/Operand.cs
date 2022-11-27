namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public struct Operand
    {
        public ushort? Number { get; }
        public string LabelRef { get; }

        public bool IsDefined => Number.HasValue;

        public Operand(ushort number, string labelRef)
        {
            Number = number;
            LabelRef = labelRef;
        }

        public Operand(ushort number)
        {
            Number = number;
            LabelRef = null;
        }

        public Operand(string labelRef)
        {
            Number = null;
            LabelRef = labelRef;
        }
    }
}