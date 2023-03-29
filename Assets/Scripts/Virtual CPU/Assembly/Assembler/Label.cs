namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Label : Symbol<ushort?>
    {
        public ushort? Address
        {
            get => Value;
            set => Value = value;
        }

        public Label(string name, string location, bool isDeclared, ushort? address) 
            : base(name, location, isDeclared, address)
        {
        }

        public override string ToString()
        {
            return $"{Name} ({(Address.HasValue ? Address.Value.ToString("X4") : "no address")})";
        }
    }
}