using System;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements
{
    public class DeclaresSymbolAttribute : Attribute
    {
        public int TokenPos { get; }

        public DeclaresSymbolAttribute(int tokenPos)
        {
            TokenPos = tokenPos;
        }
    }
}