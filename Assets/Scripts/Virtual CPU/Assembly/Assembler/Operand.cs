namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Operand
    {
        public ushort? Number { get; set; }
        public string SymbolRef { get; }
        
        public Token Token { get; }

        public bool IsDefined => Number.HasValue;
        public bool IsByte { get; set; }

        public Operand(Token token, ushort number, string symbolRef)
        {
            Token = token;
            Number = number;
            SymbolRef = symbolRef;
        }

        public Operand(Token token, ushort number)
        {
            Token = token;
            Number = number;
            SymbolRef = null;

            Token.SetMetaType(TokenMetaType.None);
        }

        public Operand(Token token, string symbolRef)
        {
            Token = token;
            Number = null;
            SymbolRef = symbolRef;
            
            // Token.SetMetaType(TokenMetaType.Label);
        }

        public Operand(Token token, ushort number, bool isByte) : this(token, number)
        {
            IsByte = isByte;
        }
        
        public Operand(Token token, string symbolRef, bool isByte) : this(token, symbolRef)
        {
            IsByte = isByte;
        }

        public override string ToString()
        {
            return IsDefined ? $"{Number:X4}" : $"[{SymbolRef}]";
        }
    }
}