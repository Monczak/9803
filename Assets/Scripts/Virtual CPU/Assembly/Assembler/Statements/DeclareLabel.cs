using System;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public sealed class DeclareLabel : IntermediateStatement
    {
        public string LabelName { get; private set; }
        
        public DeclareLabel(List<Token> tokens) : base(tokens)
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
        protected override AbstractStatement Construct(List<Token> tokens) => new DeclareLabel(tokens);
        public override List<Type> FollowedBy => new() {typeof(CreateInstruction), typeof(Directive)};
    }
}