namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Operand
    {
        public ushort? Number { get; set; }
        public string LabelRef { get; }
        
        public Token Token { get; }

        public bool IsDefined => Number.HasValue;
        public bool IsByte { get; set; }

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

            Token.SetMetaType(TokenMetaType.None);
        }

        public Operand(Token token, string labelRef)
        {
            Token = token;
            Number = null;
            LabelRef = labelRef;
            
            Token.SetMetaType(TokenMetaType.Label);
        }

        public Operand(Token token, ushort number, bool isByte) : this(token, number)
        {
            IsByte = isByte;
        }
        
        public Operand(Token token, string labelRef, bool isByte) : this(token, labelRef)
        {
            IsByte = isByte;
        }

        public override string ToString()
        {
            return IsDefined ? $"{Number:X4}" : $"[{LabelRef}]";
        }
    }
}