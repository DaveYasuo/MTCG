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
    public class Printer
    {
        // not null
        // See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving
        public static IPrinter Instance { get; private set; } = null!;
        private static bool _isSet;

        /// <summary>
        /// Use this function only once in whole solution. It initialize where to output.
        /// </summary>
        /// <param name="printer"></param>
        public static void CreateInstance(IPrinter printer)
        {
            if (_isSet) return;
            Instance = printer;
            _isSet = true;
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
