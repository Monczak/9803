using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace NineEightOhThree
{
    public static class Logger
    {
        private static readonly ReaderWriterLock FileLock = new();

        private static string logFilePath;
        private static FileStream logFileStream;

        private enum LogLevel
        {
            Info,
            Warning,
            Error,
            Fatal
        }

        public static void Setup()
        {
            string directory = Path.Combine(Application.dataPath, "Logs");
            logFilePath = Path.Combine(directory, $"{DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace(":", "-").Replace("/", "-")}.log");
            Debug.Log($"Log file path: {logFilePath}");

            if (!Application.isEditor)
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                logFileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
            }
        }

        public static void Finish()
        {
            logFileStream?.Dispose();
        }
        
        public static void Log(object message)
        {
            if (Application.isEditor) Debug.Log(message);
            else WriteToFile(message, LogLevel.Info);
        }

        public static void LogWarning(object message)
        {
            if (Application.isEditor) Debug.LogWarning(message);
            else WriteToFile(message, LogLevel.Warning);
        }

        public static void LogError(object message)
        {
            if (Application.isEditor) Debug.LogError(message);
            else WriteToFile(message, LogLevel.Error);
        }
        
        public static void LogFatal(object message)
        {
            if (Application.isEditor) Debug.LogError($"[FATAL] {message}");
            else WriteToFile(message, LogLevel.Fatal);
        }

        private static void WriteToFile(object message, LogLevel level)
        {
            try
            {
                FileLock.AcquireWriterLock(millisecondsTimeout: 30 * 1000);

                string str = GetLogLine(message, level);
                byte[] data = new UTF8Encoding(true).GetBytes(str);
                logFileStream.Write(data, 0, data.Length);
            }
            finally
            {
                FileLock.ReleaseWriterLock();
            }
        }

        private static string GetLogLine(object message, LogLevel level) =>
            $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] [{level}] {message}\n";
    }
}