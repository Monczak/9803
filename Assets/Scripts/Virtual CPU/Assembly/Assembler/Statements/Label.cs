using System;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public sealed class Label : IntermediateStatement
    {
        public string LabelName { get; private set; }
        
        public Label(List<Token> tokens) : base(tokens)
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
        protected override AbstractStatement Construct(List<Token> tokens) => new Label(tokens);
        public override List<Type> FollowedBy => new() {typeof(Instruction), typeof(Directive)};
    }
}