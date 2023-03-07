using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using UnityEngine;

namespace NineEightOhThree.Managers
{
    public static class AssemblerInterface
    {
        private static Task<Assembler.AssemblerResult> assemblerTask;

        private static Queue<Task<Assembler.AssemblerResult>> assemblerTaskQueue;

        private static Assembler assembler;
        public static Assembler Assembler
        {
            get => assembler ??= new Assembler(ErrorHandler, LogHandler);
            private set => assembler = value;
        }

        public static Assembler.AssemblerResult Assemble(string code)
        {
            return Assembler.Assemble(code);
        }

        // TODO: Implement task queue functionality
        public static void ScheduleAssembly(string code, Action<byte[], bool[]> onFinish)
        {
            assemblerTaskQueue ??= new Queue<Task<Assembler.AssemblerResult>>();

            Assembler = new Assembler(ErrorHandler, LogHandler);

            Task<Assembler.AssemblerResult>.Factory.StartNew(() => Assembler.Assemble(code))
                .ContinueWith(t =>
                {
                    var result = Assembler.OnAssemblerFinished(t);
                    onFinish(result.Code, result.CodeMask);
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static void LexicalErrorHandler(AssemblerError error)
        {
            if (error is not null) Logger.LogError($"Lexical error: {error.Message} (line {error.Line}, col {error.Column})");
        }

        private static void SyntaxErrorHandler(AssemblerError error)
        {
            if (error is not null) Logger.LogError($"Syntax error: {error.Message} ({error.Token})");
        }
        
        private static void InternalErrorHandler(AssemblerError error)
        {
            if (error is not null) Logger.LogError($"Internal error: {error.Message}");
        }

        private static void ErrorHandler(AssemblerError error)
        {
            if (error is null) return;
                
            switch (error.Type)
            {
                case AssemblerError.ErrorType.Lexical:
                    LexicalErrorHandler(error);
                    break;
                case AssemblerError.ErrorType.Syntax:
                    SyntaxErrorHandler(error);
                    break;
                case AssemblerError.ErrorType.Internal:
                    InternalErrorHandler(error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void LogHandler(string log)
        {
            Debug.Log(log);
        }
    }
}