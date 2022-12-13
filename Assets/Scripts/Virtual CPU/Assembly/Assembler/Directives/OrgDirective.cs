using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public sealed class OrgDirective : Directive
    {
        public OrgDirective(List<Operand> operands) : base(operands)
        {
        }

        public override string Name => "org";
        public override DirectiveType Type => DirectiveType.Variadic;
        public override int ArgCount => 1;

        public override OperationResult<List<byte>> Evaluate(ref ushort programCounter)
        {
            if (operands.Count != 1)
                return OperationResult<List<byte>>.Error(SyntaxErrors.Expected(null, TokenType.Number));

            if (operands[0].IsDefined)
            {
                var number = operands[0].Number;
                if (number != null)
                    programCounter = number.Value;
                return OperationResult<List<byte>>.Success(null);
            }
            else
                return OperationResult<List<byte>>.Error(SyntaxErrors.ExpectedGot(operands[0].Token, TokenType.Number, operands[0].Token.Type));
        }

        protected internal override Directive Construct(List<Operand> ops) => new OrgDirective(ops);
    }
}