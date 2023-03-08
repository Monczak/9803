using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public delegate void ErrorHandler(AssemblerError ex);

    public delegate void LogHandler(string message);
    
    public class Assembler
    {
        public Lexer Lexer { get; }
        public Parser Parser { get; }
        public CodeGenerator CodeGenerator { get; }

        private List<Token> tokens;
        private List<AbstractStatement> statements;

        private readonly ErrorHandler errorHandler;
        private readonly LogHandler logHandler;

        public Assembler(ErrorHandler errorHandler, LogHandler logHandler)
        {
            Lexer = new Lexer();
            Parser = new Parser();
            CodeGenerator = new CodeGenerator();

            this.errorHandler = errorHandler;
            this.logHandler = logHandler;
        }
        
        public AssemblerResult Assemble(string input, string fileName, bool writeResetVector = true)
        {
            tokens = new List<Token>();
            statements = new List<AbstractStatement>();

            List<string> logs = new();
            List<AssemblerError> errors = new();

            void AddError(AssemblerError e) => errors.Add(e);
            void AddLog(string log) => logs.Add(log);

            Lexer.RegisterErrorHandler(AddError);
            Lexer.RegisterLogHandler(AddLog);
            Parser.RegisterErrorHandler(AddError);
            Parser.RegisterLogHandler(AddLog);
            CodeGenerator.RegisterErrorHandler(AddError);
            CodeGenerator.RegisterLogHandler(AddLog);

            AssembledCode assembledCode = null;
            try
            {
                tokens = Lexer.Lex(input, fileName);
                statements = Parser.Parse(tokens);
                assembledCode = CodeGenerator.GenerateCode(statements);

                if (writeResetVector) assembledCode.WriteResetVector();
                assembledCode.WriteIrqVector();
                assembledCode.WriteNmiVector();
            }
            catch (Exception e)
            {
                errors.Add(new AssemblerError(AssemblerError.ErrorType.Internal, e.Message + e.StackTrace, null));
            }

            return new AssemblerResult(assembledCode, logs, errors, tokens, statements);
        }

        public AssemblerResult OnAssemblerFinished(Task<AssemblerResult> task)
        {
            foreach (var error in task.Result.Errors)
                errorHandler(error);
            foreach (var log in task.Result.Logs)
                logHandler(log);
            LogDebugData();
            return task.Result;
        }

        public void LogDebugData()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Token token in tokens)
                builder.Append("(").Append(token).Append(") ");
            Logger.Log(builder.ToString());
            
            builder = new StringBuilder();
            foreach (AbstractStatement statement in statements)
                builder.Append("(").Append(statement is null ? "invalid statement" : statement.GetType().Name).Append(") ");
            Logger.Log(builder.ToString());
        }
    }
}