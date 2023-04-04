using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    [DeclaresSymbol(tokenPos: 0)]
    public sealed class ConstantStatement : FinalStatement
    {
        public string ConstantName { get; private set; }
        public ushort ConstantValue { get; private set; }

        public ConstantStatement(List<Token> tokens) : base(tokens)
        {
            
        }

        protected internal override List<(NodePattern pattern, TokenHandler handler)> Pattern => new()
        {
            (NodePattern.Single(TokenType.Identifier), token =>
            {
                ConstantName = token.Content;
                token.MetaType = TokenMetaType.Constant;
                return OperationResult.Success();
            }),
            (NodePattern.Single(TokenType.Equals), null),
            (NodePattern.Single(TokenType.Number), token =>
            {
                ConstantValue = (ushort)token.Literal;
                return OperationResult.Success();
            })
        };

        protected override AbstractStatement Construct(List<Token> tokens) => new ConstantStatement(tokens);
    }
}