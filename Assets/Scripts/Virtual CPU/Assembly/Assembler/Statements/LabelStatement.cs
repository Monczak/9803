using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    [DeclaresSymbol(tokenPos: 0)]
    public sealed class LabelStatement : IntermediateStatement
    {
        public string LabelName { get; private set; }

        public LabelStatement(List<Token> tokens) : base(tokens)
        {
            
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.LabelDecl), token =>
            {
                LabelName = token.Content[..^1];
                return OperationResult.Success();
            })
        };
        protected override AbstractStatement Construct(List<Token> tokens) => new LabelStatement(tokens);
        public override List<Type> FollowedBy => new() {typeof(InstructionStatement), typeof(DirectiveStatement)};
    }
}