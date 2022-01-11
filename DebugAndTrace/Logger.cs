using System;
using System.Diagnostics;

namespace DebugAndTrace
{
    public interface IPrinter
    {
        void WriteLine(object text);
    }
    // Singleton-pattern
    // See: "https://de.wikipedia.org/wiki/Liste_von_Singleton-Implementierungen#Implementierung_in_C#"
    // Somewhat of a Factory pattern? 
    // See: https://www.dofactory.com/net/factory-method-design-pattern
    // Wrapper Class for Printers
    public class Logger
    {
        public static IPrinter GetPrinter(Printer printer)
        {
            return printer switch
            {
                Printer.Console => ConsolePrinter.Instance,
                Printer.Debug => DebugPrinter.Instance,
                Printer.Trace => TracePrinter.Instance,
                _ => throw new ArgumentOutOfRangeException(nameof(printer), printer, null)
            };
        }
    }
    public class DebugPrinter : IPrinter
    {
        public static readonly DebugPrinter Instance = new();

        private DebugPrinter()
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
    public class ConsolePrinter : IPrinter
    {
        public static readonly ConsolePrinter Instance = new();

        private ConsolePrinter()
        {
            Debug.WriteLine("Using Console Printer");
        }
        public void WriteLine(object text)
        {
            Console.WriteLine(text);
        }
    }
    public class TracePrinter : IPrinter
    {
        public static readonly TracePrinter Instance = new();

        private TracePrinter()
        {
            Debug.WriteLine("Using Trace Printer");
        }
        public void WriteLine(object text)
        {
            Trace.WriteLine(text);
        }
    }
}
