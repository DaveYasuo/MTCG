using System;
using System.Diagnostics;

namespace DebugAndTrace
{
    public interface IPrinter
    {
        void WriteLine(object text);
    }
    public class DebugPrinter : IPrinter
    {
        public void WriteLine(object text)
        {
            Debug.Indent();
            Debug.WriteLine(text);
            Debug.Unindent();
        }
    }
    public class ConsolePrinter : IPrinter
    {
        public void WriteLine(object text)
        {
            Console.WriteLine(text);
        }
    }
    public class TracePrinter : IPrinter
    {
        public void WriteLine(object text)
        {
            Trace.WriteLine(text);
        }
    }
}
