using System;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public class NodePattern
    {
        public TokenType? TokenType { get; }
        public bool Cycle { get; }

        private NodePattern(TokenType type, bool cycle)
        {
            TokenType = type;
            Cycle = cycle;
        }

        public static NodePattern Single(TokenType type) => new(type, false);
        public static NodePattern Multiple(TokenType type) => new(type, true);

        private bool Equals(NodePattern other)
        {
            return TokenType == other.TokenType && Cycle == other.Cycle;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((NodePattern)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TokenType, Cycle);
        }
    }
}