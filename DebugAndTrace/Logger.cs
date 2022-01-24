using System;
using System.Diagnostics;

namespace DebugAndTrace
{
    // Singleton-pattern
    // See: "https://de.wikipedia.org/wiki/Liste_von_Singleton-Implementierungen#Implementierung_in_C#"
    // Somewhat of a Factory pattern? 
    // See: https://www.dofactory.com/net/factory-method-design-pattern
    // Wrapper Class for Printers
    public class Logger
    {
        public static ILogger GetPrinter(Printer printer)
        {
            return printer switch
            {
                Printer.Console => ConsoleLogger.Instance,
                Printer.Debug => DebugLogger.Instance,
                Printer.Trace => TraceLogger.Instance,
                _ => throw new ArgumentOutOfRangeException(nameof(printer), printer, null)
            };
        }
    }

    public class DebugLogger : ILogger
    {
        public static readonly DebugLogger Instance = new();

        private DebugLogger()
        {
            Debug.WriteLine("Using Debug Printer");
        }

        public void WriteLine(object text)
        {
            Debug.Indent();
            Debug.WriteLine(text);
            Debug.Unindent();
        }
    }

    public class ConsoleLogger : ILogger
    {
        public static readonly ConsoleLogger Instance = new();

        private ConsoleLogger()
        {
            Debug.WriteLine("Using Console Printer");
        }

        public void WriteLine(object text)
        {
            Console.WriteLine(text);
        }
    }

    public class TraceLogger : ILogger
    {
        public static readonly TraceLogger Instance = new();

        private TraceLogger()
        {
            Debug.WriteLine("Using Trace Printer");
        }

        public void WriteLine(object text)
        {
            Trace.WriteLine(text);
        }
    }
}