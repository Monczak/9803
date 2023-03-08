using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public sealed class BeginDirective : Directive
    {
        public BeginDirective(List<Operand> operands) : base(operands)
        {
        }

        public override string Name => "begin";
        public override DirectiveType Type => DirectiveType.Nullary;
        public override int ArgCount => 0;

        public override bool Single => true;

        public override OperationResult<List<Operand>> Evaluate(ref ushort programCounter, Vectors vectors)
        {
            vectors.Reset = programCounter;
            return OperationResult<List<Operand>>.Success(null);
        }

        protected internal override Directive Construct(List<Operand> ops) => new BeginDirective(ops);
    }
}