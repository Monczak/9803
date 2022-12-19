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
        private readonly Lexer lexer;
        private readonly Parser parser;
        private readonly CodeGenerator codeGenerator;

        private List<Token> tokens;
        private List<AbstractStatement> statements;

        private readonly ErrorHandler errorHandler;
        private readonly LogHandler logHandler;

        public struct AssemblerResult
        {
            public byte[] Code { get; }
            public bool[] CodeMask { get; }
            public List<string> Logs { get; }
            public List<AssemblerError?> Errors { get; }

            public AssemblerResult(byte[] code, bool[] codeMask, List<string> logs, List<AssemblerError?> errors)
            {
                Code = code;
                CodeMask = codeMask;
                Logs = logs;
                Errors = errors;
            }
        }

        public Assembler(ErrorHandler errorHandler, LogHandler logHandler)
        {
            lexer = new Lexer();
            parser = new Parser();
            codeGenerator = new CodeGenerator();

            this.errorHandler = errorHandler;
            this.logHandler = logHandler;
        }
        
        public Func<AssemblerResult> Assemble(string input)
        {
            tokens = new List<Token>();
            statements = new List<AbstractStatement>();

            return () =>
            {
                List<string> logs = new();
                List<AssemblerError?> errors = new();

                void AddError(AssemblerError? e) => errors.Add(e);
                void AddLog(string log) => logs.Add(log);

                lexer.RegisterErrorHandler(AddError);
                lexer.RegisterLogHandler(AddLog);
                parser.RegisterErrorHandler(AddError);
                parser.RegisterLogHandler(AddLog);
                codeGenerator.RegisterErrorHandler(AddError);
                codeGenerator.RegisterLogHandler(AddLog);

                byte[] code = null;
                bool[] codeMask = null;
                try
                {
                    tokens = lexer.Lex(input);
                    statements = parser.Parse(tokens);
                    (code, codeMask) = codeGenerator.GenerateCode(statements);
                }
                catch (Exception e)
                {
                    errors.Add(new AssemblerError(AssemblerError.ErrorType.Internal, e.Message + e.StackTrace, null));
                }

                return new AssemblerResult(code, codeMask, logs, errors);
            };
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
            Debug.Log(builder.ToString());
            
            builder = new StringBuilder();
            foreach (AbstractStatement statement in statements)
                builder.Append("(").Append(statement is null ? "invalid statement" : statement.GetType().Name).Append(") ");
            Debug.Log(builder.ToString());
        }
    }
}