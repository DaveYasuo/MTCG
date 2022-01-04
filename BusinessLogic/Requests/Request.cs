using System;
using System.IO;

namespace BusinessLogic.Requests
{
    public class Request : IRequestReader
    {
        public string GetRequestString { get; }


        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public Request(in StreamReader dataStream)
        {
            string line;
            while (!string.IsNullOrWhiteSpace(line = dataStream.ReadLine()))
            {
                GetRequestString += line + Environment.NewLine;
            }
            Console.WriteLine(GetRequestString);
        }

    }
}