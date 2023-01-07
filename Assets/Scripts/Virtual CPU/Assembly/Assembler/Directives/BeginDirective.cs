using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public class BeginDirective : Directive
    {
        public BeginDirective(List<Operand> operands) : base(operands)
        {
        }

        public override string Name => "begin";
        public override DirectiveType Type => DirectiveType.Nullary;
        public override int ArgCount => 0;
        public override OperationResult<List<Operand>> Evaluate(ref ushort programCounter)
        {
            return OperationResult<List<Operand>>.Success(null);
        }

        protected internal override Directive Construct(List<Operand> ops) => new BeginDirective(ops);
    }
}