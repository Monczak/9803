using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public sealed class ByteDirective : Directive
    {
        public ByteDirective(List<Operand> operands) : base(operands)
        {
        }

        public override string Name => "byte";
        public override DirectiveType Type => DirectiveType.Variadic;
        public override int ArgCount => -1;

        public override OperationResult<List<byte>> Evaluate(ref ushort programCounter)
        {
            List<byte> bytes = new();
            foreach (Operand op in operands)
            {
                if (op.IsDefined && op.Number <= 0xFF)
                    bytes.Add((byte)op.Number);
                else
                    return OperationResult<List<byte>>.Error(SyntaxErrors.OperandNotByte(op.Token));
            }

            return OperationResult<List<byte>>.Success(bytes);
        }

        protected internal override Directive Construct(List<Operand> ops) => new ByteDirective(ops);
    }
}