namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Label : Symbol<ushort?>
    {
        public ushort? Address
        {
            get => Value;
            set => Value = value;
        }

        public Label(string name, string location, ushort? address, bool isDeclared)
        {
            Name = name;
            Location = location;
            Address = address;
            IsDeclared = isDeclared;
        }

        public override string ToString()
        {
            return $"{Name} ({(Address.HasValue ? Address.Value.ToString("X4") : "no address")})";
        }
    }
}