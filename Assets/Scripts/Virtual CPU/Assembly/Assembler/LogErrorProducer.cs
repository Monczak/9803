using System;
using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.VirtualCPU.Assembly.Build;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public abstract class LogErrorProducer
    {
        protected HashSet<LogHandler> LogHandlers { get; private set; }
        protected event LogHandler OnLog;
        private event LogHandler OnLogSubscribeOnce
        {
            add
            {
                LogHandlers ??= new HashSet<LogHandler>();
                LogHandlers.Add(value);
                
                OnLog -= value;
                OnLog += value;
            }
            remove
            {
                LogHandlers.Remove(value);
                OnLog -= value;
            }
        }

        protected HashSet<ErrorHandler> ErrorHandlers { get; private set; }
        protected event ErrorHandler OnError;
        private event ErrorHandler OnErrorSubscribeOnce
        {
            add
            {
                ErrorHandlers ??= new HashSet<ErrorHandler>();
                ErrorHandlers.Add(value);
                
                OnError -= value;
                OnError += value;
            }
            remove
            {
                ErrorHandlers.Remove(value);
                OnError -= value;
            }
        }

        public void RegisterLogHandler(LogHandler handler) => OnLogSubscribeOnce += handler;

        public void RegisterLogHandlers(IEnumerable<LogHandler> handlers)
        {
            foreach (var handler in handlers) RegisterLogHandler(handler);
        }

        public void UnregisterLogHandler(LogHandler handler) => OnLogSubscribeOnce -= handler;

        protected void MakeLog(string log) => OnLog?.Invoke(log);

        public void RegisterErrorHandler(ErrorHandler handler) => OnErrorSubscribeOnce += handler;

        public void RegisterErrorHandlers(IEnumerable<ErrorHandler> handlers)
        {
            foreach (var handler in handlers) RegisterErrorHandler(handler);
        }
        
        public void UnregisterErrorHandler(ErrorHandler handler)
        {
            OnErrorSubscribeOnce -= handler;

            ErrorHandlers.Remove(handler);
        }

        protected void MakeError(AssemblerError error) => OnError?.Invoke(error);
    }
}