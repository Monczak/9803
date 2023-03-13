using System;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public sealed class IncludeDirective : Directive
    {
        public string IncludedResourceLocation { get; private set; }
        
        public IncludeDirective(List<Operand> operands) : base(operands)
        {
        }

        public override string Name => "include";
        public override DirectiveType Type => DirectiveType.Variadic;
        public override int ArgCount => 1;

        public void Setup(string includedResourceLocation)
        {
            IncludedResourceLocation = includedResourceLocation;
        }
        
        public override OperationResult<List<Operand>> Evaluate(ref ushort programCounter, Vectors vectors)
        {
            return OperationResult<List<Operand>>.Success(null);
        }

        protected internal override Directive Construct(List<Operand> ops) => new IncludeDirective(ops);
    }
}