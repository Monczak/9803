namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Label
    {
        public string Name { get; }
        public string Location { get; }
        public ushort? Address { get; set; }

        public bool IsDeclared { get; set; }

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