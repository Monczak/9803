using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Assembly;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public abstract class Directive
    {
        public abstract string Name { get; }
        public abstract DirectiveType Type { get; }
        public abstract int ArgCount { get; }
        
        public virtual bool Single { get; } = false;

        protected readonly List<Operand> operands;

        protected const int UnlimitedArgs = -1;

        public abstract OperationResult<List<Operand>> Evaluate(ref ushort programCounter, Vectors vectors);
        
        protected Directive(List<Operand> operands)
        {
            this.operands = operands is null ? null : new List<Operand>(operands);
        }

        protected internal abstract Directive Construct(List<Operand> ops);

        public OperationResult<Directive> Build(List<Operand> ops)
        {
            if (Type == DirectiveType.Variadic && (ops is null || ops.Count == 0))
                return OperationResult<Directive>.Error(SyntaxErrors.WrongArgumentCount(null, ArgCount));
            
            if (Type == DirectiveType.Nullary && (ops is not null && ops.Count != 0))
                return OperationResult<Directive>.Error(SyntaxErrors.WrongArgumentCount(null, 0));
            
            if (ArgCount != -1 && (ops is not null && ops.Count != ArgCount))
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