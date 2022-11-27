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

        protected internal override List<(TokenType type, TokenHandler handler)> Pattern => new()
        {
            (TokenType.LabelDecl, token =>
            {
                LabelName = token.Content[..^1];
            })
        };
        protected override AbstractStatement Construct(List<Token> tokens) => new DeclareLabel(tokens);
        public override Type FollowedBy => typeof(CreateInstruction);
    }
}