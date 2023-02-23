using System.Diagnostics;

namespace DotEngine;

public static class Loger
{
    public enum Type
    {
        Debug, Error, Info
    }

    public static string Address { get; set; } = "./log.log";
  
    private static Queue<LogMessage> LogQueue = new Queue<LogMessage>();

    private static readonly object WriteLock = new object();

    public static LogMessage Logging(Type type, string message, bool writeOnFile = false)
    {
        var logMessage = new LogMessage(type, message);

        Debug.Write(logMessage.ToString());
        Debug.Print(logMessage.ToString());

        LogQueue.Enqueue(logMessage);

        if (writeOnFile)
        {
            lock (WriteLock)
            {
                using (var writer = new StreamWriter(Address,append:true))
                {
                    while (LogQueue.Count > 0)
                    {
                        var log = LogQueue.Dequeue();

                        writer.WriteLine(log.ToString());
                    }
                }
            }
        }

        return logMessage;
    }

    public class LogMessage
    {
        public Type Type { get; private set; }

        public string Message { get; private set; }

        public LogMessage(Type type, string message)
        {
            Type = type;
            Message = message;
        }

        public override string ToString() =>
            $"[{DateTime.Now:yyyy-MM-dd-HH-mm-ss}][{Type}]:{Message}";
    }
}

