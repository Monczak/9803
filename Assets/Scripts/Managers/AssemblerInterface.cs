using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using UnityEngine;

namespace NineEightOhThree.Managers
{
    public static class AssemblerInterface
    {
        private static Task<AssemblerResult> assemblerTask;

        private static Queue<Task<AssemblerResult>> assemblerTaskQueue;

        private static Assembler assembler;

        private const string ResourceLocation = "Int";
        
        public static Assembler Assembler
        {
            get => assembler ??= new Assembler(HandleErrors, HandleLogs);
            private set => assembler = value;
        }

        public static AssemblerResult Assemble(string code, string resourceLocation = ResourceLocation)
        {
            return Assemble(code, HandleErrors, HandleLogs, resourceLocation);
        }
        
        public static AssemblerResult Assemble(string code, ErrorHandler errorHandler, LogHandler logHandler, string resourceLocation = ResourceLocation)
        {
            Assembler = new Assembler(errorHandler, logHandler);
            return Assembler.Assemble(code, resourceLocation);
        }

        // TODO: Implement task queue functionality
        public static void ScheduleAssembly(string code, Action<byte[], bool[]> onFinish)
        {
            assemblerTaskQueue ??= new Queue<Task<AssemblerResult>>();

            Assembler = new Assembler(HandleErrors, HandleLogs);

            Task<AssemblerResult>.Factory.StartNew(() => Assembler.Assemble(code, ResourceLocation))
                .ContinueWith(t =>
                {
                    var result = Assembler.OnAssemblerFinished(t);
                    onFinish(result.AssembledCode.Code, result.AssembledCode.Mask);
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

        private static void HandleErrors(AssemblerError error)
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

        private static void HandleLogs(string log)
        {
            Logger.Log(log);
        }
    }
}