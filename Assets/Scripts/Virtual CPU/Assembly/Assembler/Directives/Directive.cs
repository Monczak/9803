using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public abstract class Directive
    {
        public abstract string Name { get; }
        public abstract DirectiveType Type { get; }
        public abstract int ArgCount { get; }
        
        protected readonly List<Operand> operands;

        public abstract OperationResult<List<Operand>> Evaluate(ref ushort programCounter);
        
        protected Directive(List<Operand> operands)
        {
            this.operands = operands is null ? null : new List<Operand>(operands);
        }

        protected internal abstract Directive Construct(List<Operand> ops);

        public OperationResult<Directive> Build(List<Operand> ops)
        {
            if (Type == DirectiveType.Variadic && (ops is null || ops.Count == 0))
                return OperationResult<Directive>.Error(SyntaxErrors.WrongArgumentCount(null, ArgCount));
            
            if (Type == DirectiveType.Nullary && ops.Count != 0)
                return OperationResult<Directive>.Error(SyntaxErrors.WrongArgumentCount(null, 0));
            
            if (ArgCount != -1 && ops.Count != ArgCount)
                return OperationResult<Directive>.Error(SyntaxErrors.WrongArgumentCount(null, ArgCount));
            
            Directive directive = Construct(ops);
            return OperationResult<Directive>.Success(directive);
        }
    }

    public enum DirectiveType
    {
        Nullary,
        Variadic
    }
}