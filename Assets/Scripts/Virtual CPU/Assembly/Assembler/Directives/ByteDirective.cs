using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Assembly;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public sealed class ByteDirective : Directive
    {
        public ByteDirective(List<Operand> operands) : base(operands)
        {
        }

        public override string Name => "byte";
        public override DirectiveType Type => DirectiveType.Variadic;
        public override int ArgCount => UnlimitedArgs;

        public override OperationResult<List<Operand>> Evaluate(ref ushort programCounter, Vectors vectors)
        {
            foreach (Operand op in operands)
            {
                if (!(op.IsDefined && op.Number <= 0xFF))
                    return OperationResult<List<Operand>>.Error(SyntaxErrors.OperandNotByte(op.Token));
                op.IsByte = true;
            }

            return OperationResult<List<Operand>>.Success(operands);
        }

        protected internal override Directive Construct(List<Operand> ops) => new ByteDirective(ops);
    }
}