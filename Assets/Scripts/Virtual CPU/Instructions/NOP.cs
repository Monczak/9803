namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class NOP : CPUInstruction
    {
        public override string Mnemonic => "NOP";
        public override byte Opcode => 0x00;
        public override int ArgumentCount => 0;

        public override void Execute(params byte[] @params)
        {
            // Do nothing
        }
    }
}
