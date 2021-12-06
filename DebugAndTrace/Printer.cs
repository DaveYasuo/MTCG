using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugAndTrace
{
    //public IPrinter Printer { get; }
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
}
