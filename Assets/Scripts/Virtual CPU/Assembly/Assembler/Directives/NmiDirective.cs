using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public class NmiDirective : Directive
    {
        public NmiDirective(List<Operand> operands) : base(operands)
        {
        }

        public override string Name => "nmi";
        public override DirectiveType Type => DirectiveType.Nullary;
        public override int ArgCount => 0;
        public override bool Single => true;
        
        public override OperationResult<List<Operand>> Evaluate(ref ushort programCounter, Vectors vectors)
        {
            vectors.Nmi = programCounter;
            return OperationResult<List<Operand>>.Success(null);
        }

        protected internal override Directive Construct(List<Operand> ops) => new NmiDirective(ops);
    }
}