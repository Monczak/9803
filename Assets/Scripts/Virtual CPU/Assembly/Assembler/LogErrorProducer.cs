using System;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public abstract class LogErrorProducer
    {
        protected event LogHandler OnLog;
        private event LogHandler OnLogSubscribeOnce
        {
            add
            {
                OnLog -= value;
                OnLog += value;
            }
            remove => OnLog -= value;
        }
        
        protected event ErrorHandler OnError;
        private event ErrorHandler OnErrorSubscribeOnce
        {
            add
            {
                OnError -= value;
                OnError += value;
            }
            remove => OnError -= value;
        }
        
        public void RegisterLogHandler(LogHandler handler) => OnLogSubscribeOnce += handler;

        public void UnregisterLogHandler(LogHandler handler) => OnLogSubscribeOnce -= handler;

        protected void MakeLog(string log) => OnLog?.Invoke(log);
        
        public void RegisterErrorHandler(ErrorHandler handler) => OnErrorSubscribeOnce += handler;
        public void UnregisterErrorHandler(ErrorHandler handler) => OnErrorSubscribeOnce -= handler;

        protected void MakeError(AssemblerError error) => OnError?.Invoke(error);
    }
}