namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Operand
    {
        public ushort? Number { get; set; }
        public string LabelRef { get; }
        
        public Token Token { get; }

        public bool IsDefined => Number.HasValue;

        public Operand(Token token, ushort number, string labelRef)
        {
            Token = token;
            Number = number;
            LabelRef = labelRef;
        }

        public Operand(Token token, ushort number)
        {
            Token = token;
            Number = number;
            LabelRef = null;
        }

        public Operand(Token token, string labelRef)
        {
            Token = token;
            Number = null;
            LabelRef = labelRef;
        }

        public override string ToString()
        {
            return IsDefined ? $"{Number:X4}" : $"[{LabelRef}]";
        }
    }
}