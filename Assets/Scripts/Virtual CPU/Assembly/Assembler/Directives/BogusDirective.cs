using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Assembly;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public sealed class BogusDirective : Directive
    {
        public BogusDirective(List<Operand> operands) : base(operands)
        {
        }

        public override string Name => "bogus";
        public override DirectiveType Type => DirectiveType.Nullary;
        public override int ArgCount => 0;
        public override OperationResult<List<Operand>> Evaluate(ref ushort programCounter)
        {
            return OperationResult<List<Operand>>.Success(null);
        }

        protected internal override Directive Construct(List<Operand> ops) => new BogusDirective(ops);
    }
}