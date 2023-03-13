using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public class WordDirective : Directive
    {
        public WordDirective(List<Operand> operands) : base(operands)
        {
        }

        public override string Name => "word";
        public override DirectiveType Type => DirectiveType.Variadic;
        public override int ArgCount => UnlimitedArgs;
        
        public override OperationResult<List<Operand>> Evaluate(ref ushort programCounter, Vectors vectors)
        {
            return OperationResult<List<Operand>>.Success(operands);
        }

        protected internal override Directive Construct(List<Operand> ops) => new WordDirective(ops);
    }
}