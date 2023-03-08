using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class AssemblerResult
    {
        public AssembledCode AssembledCode { get; }
        public List<string> Logs { get; }
        public List<AssemblerError> Errors { get; }
            
        public List<Token> Tokens { get; }
        public List<AbstractStatement> Statements { get; }

        public AssemblerResult(AssembledCode assembledCode, List<string> logs, List<AssemblerError> errors, List<Token> tokens, List<AbstractStatement> statements)
        {
            AssembledCode = assembledCode;
            Logs = logs;
            Errors = errors;
            Tokens = tokens;
            Statements = statements;
        }
    }
}