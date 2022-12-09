namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Label
    {
        public string Name { get; set; }
        public ushort? Address { get; set; }

        public bool IsDeclared { get; set; }

        public Label(string name, ushort? address, bool isDeclared)
        {
            Name = name;
            Address = address;
            IsDeclared = isDeclared;
        }

        public override string ToString()
        {
            return $"{Name} ({(Address.HasValue ? Address.Value.ToString("X4") : "no address")})";
        }
    }
}