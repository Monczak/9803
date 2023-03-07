using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public delegate void ErrorHandler(AssemblerError? ex);

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

        public class AssemblerResult
        {
            public byte[] Code { get; }
            public bool[] CodeMask { get; }
            public List<string> Logs { get; }
            public List<AssemblerError?> Errors { get; }
            
            public List<Token> Tokens { get; }
            public List<AbstractStatement> Statements { get; }

            public AssemblerResult(byte[] code, bool[] codeMask, List<string> logs, List<AssemblerError?> errors, List<Token> tokens, List<AbstractStatement> statements)
            {
                Code = code;
                CodeMask = codeMask;
                Logs = logs;
                Errors = errors;
                Tokens = tokens;
                Statements = statements;
            }
        }

        public Assembler(ErrorHandler errorHandler, LogHandler logHandler)
        {
            Lexer = new Lexer();
            Parser = new Parser();
            CodeGenerator = new CodeGenerator();

            this.errorHandler = errorHandler;
            this.logHandler = logHandler;
        }
        
        public AssemblerResult Assemble(string input)
        {
            tokens = new List<Token>();
            statements = new List<AbstractStatement>();

            List<string> logs = new();
            List<AssemblerError?> errors = new();

            void AddError(AssemblerError? e) => errors.Add(e);
            void AddLog(string log) => logs.Add(log);

            Lexer.RegisterErrorHandler(AddError);
            Lexer.RegisterLogHandler(AddLog);
            Parser.RegisterErrorHandler(AddError);
            Parser.RegisterLogHandler(AddLog);
            CodeGenerator.RegisterErrorHandler(AddError);
            CodeGenerator.RegisterLogHandler(AddLog);

            byte[] code = null;
            bool[] codeMask = null;
            try
            {
                tokens = Lexer.Lex(input);
                statements = Parser.Parse(tokens);
                (code, codeMask) = CodeGenerator.GenerateCode(statements);
            }
            catch (Exception e)
            {
                errors.Add(new AssemblerError(AssemblerError.ErrorType.Internal, e.Message + e.StackTrace, null));
            }

            return new AssemblerResult(code, codeMask, logs, errors, tokens, statements);
        }

        public AssemblerResult OnAssemblerFinished(Task<AssemblerResult> task)
        {
            foreach (var error in task.Result.Errors)
                errorHandler(error);
            foreach (var log in task.Result.Logs)
                logHandler(log);
            LogAssemblerThings();
            return task.Result;
        }

        private void LogAssemblerThings()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Token token in tokens)
                builder.Append("(").Append(token.ToString()).Append(") ");
            Logger.Log(builder.ToString());
            
            builder = new StringBuilder();
            foreach (AbstractStatement statement in statements)
                builder.Append("(").Append(statement is null ? "invalid statement" : statement.GetType().Name).Append(") ");
            Logger.Log(builder.ToString());
        }
    }
}