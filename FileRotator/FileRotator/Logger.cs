using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileRotator
{
    public class LineLoggedEventArgs
    {
        public LineLoggedEventArgs(string s, ConsoleColor c, bool b) { Text = s; ConsoleColor = c; Center = b; }
        public String Text { get; private set; }
        public ConsoleColor ConsoleColor { get; private set; }
        public bool Center { get; private set; }
    }
    public class Logger
    {
        public Logger(int Verbosity, string LogLocation)
        {
            verbosity = Verbosity;
            loglocation = LogLocation;
        }
        int verbosity { get; set; }
        string loglocation { get; set; }
        private string[] statusCodes = { "INFO", "WARN", "ERROR" };
        private ConsoleColor[] colorScale = { ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Red };

        public delegate void LineLoggedEventHandler(object sender, LineLoggedEventArgs e);
        public event LineLoggedEventHandler LineLogged;
        protected virtual void RaiseLineLoggedEvent(string message, ConsoleColor color, bool center)
        {
            if (LineLogged != null)
                LineLogged.Invoke(this, new LineLoggedEventArgs(message, color, center));
        }
        public void LogLine(string Message)
        {
            if (0 >= verbosity)
                WriteToLogFile(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[0] + ": " + Message);
            RaiseLineLoggedEvent(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[0] + ": " + Message, colorScale[0], false);
        }
        public void LogLine(int StatusCode, string Message)
        {
            if (StatusCode >= verbosity)
                WriteToLogFile(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[StatusCode] + ": " + Message);
            RaiseLineLoggedEvent(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[StatusCode] + ": " + Message, colorScale[StatusCode], false);
        }
        public void LogLine(int StatusCode, string Message, bool dispEvnt)
        {
            if (StatusCode >= verbosity)
                WriteToLogFile(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[StatusCode] + ": " + Message);
            if (dispEvnt)
                RaiseLineLoggedEvent(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[StatusCode] + ": " + Message, colorScale[StatusCode], false);
            else
                RaiseLineLoggedEvent(Message, colorScale[StatusCode], false);

        }
        public void LogLine(string Message, bool dispEvnt)
        {
            if (0 >= verbosity)
                WriteToLogFile(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[0] + ": " + Message);
            if (dispEvnt)
                RaiseLineLoggedEvent(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[0] + ": " + Message, colorScale[0], false);
            else
                RaiseLineLoggedEvent(Message, colorScale[0], false);

        }
        public void LogLine(string Message, bool dispEvnt, bool centerText)
        {
            if (0 >= verbosity)
                WriteToLogFile(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[0] + ": " + Message);
            if (dispEvnt)
                RaiseLineLoggedEvent(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[0] + ": " + Message, colorScale[0], centerText);
            else
                RaiseLineLoggedEvent(Message, colorScale[0], centerText);

        }
        public void LogLine(int StatusCode, string Message, bool dispEvnt, bool centerText)
        {
            if (StatusCode >= verbosity)
                WriteToLogFile(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[StatusCode] + ": " + Message);
            if (dispEvnt)
                RaiseLineLoggedEvent(DateTime.Now.ToString("[HH:mm:ss]") + " " + statusCodes[StatusCode] + ": " + Message, colorScale[StatusCode], centerText);
            else
                RaiseLineLoggedEvent(Message, colorScale[StatusCode], centerText);

        }
        bool writing;
        private void WriteToLogFile(string message)
        {
            while (writing)
                System.Threading.Thread.Sleep(1);
            writing = true;
            if (!Directory.Exists(loglocation))
                Directory.CreateDirectory(loglocation);
            using (StreamWriter sw = File.AppendText(loglocation + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log"))
            {
                sw.WriteLine(message);
            }
            writing = false; //FIX THIS
        }
    }
}
