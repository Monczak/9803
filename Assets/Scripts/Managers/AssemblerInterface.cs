﻿using System;
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

        // TODO: Implement task queue functionality
        public static void ScheduleAssembly(string code, Action<Assembler.AssemblerResult> onFinish)
        {
            assemblerTaskQueue ??= new Queue<Task<Assembler.AssemblerResult>>();

            Assembler = new Assembler(ErrorHandler, LogHandler);
            var assemble = Assembler.Assemble(code);

            Task<Assembler.AssemblerResult>.Factory.StartNew(assemble)
                .ContinueWith(t =>
                {
                    Assembler.AssemblerResult result = Assembler.OnAssemblerFinished(t);
                    onFinish(result);
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static void LexicalErrorHandler(AssemblerError? error)
        {
            if (error != null) Logger.LogError($"Lexical error: {error.Value.Message} (line {error.Value.Line}, col {error.Value.Column})");
        }

        private static void SyntaxErrorHandler(AssemblerError? error)
        {
            if (error != null) Logger.LogError($"Syntax error: {error.Value.Message} ({error.Value.Token})");
        }
        
        private static void InternalErrorHandler(AssemblerError? error)
        {
            if (error != null) Logger.LogError($"Internal error: {error.Value.Message}");
        }

        private static void ErrorHandler(AssemblerError? error)
        {
            if (error == null) return;
                
            switch (error.Value.Type)
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