using System;

namespace YtbDownloader.Core.Common
{
    public class LogReceivedEventArgs : EventArgs
    {
        public string EventMessage { get; }
        public DateTime EventDateTime { get; }

        public LogReceivedEventArgs(string message)
        {
            EventMessage = message;
            EventDateTime = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{DateTime.Now} {EventMessage}";
        }
    }
}