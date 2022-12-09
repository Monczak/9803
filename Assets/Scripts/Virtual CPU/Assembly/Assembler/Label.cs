namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Label
    {
        public string Name { get; set; }
        public ushort? Address { get; set; }

        public bool IsDefined => Address is not null;
        
        public Label(string name, ushort? address)
        {
            Name = name;
            Address = address;
        }

        public override string ToString()
        {
            return $"{Name} ({(Address.HasValue ? Address.Value.ToString("X4") : "no address")})";
        }
    }
}