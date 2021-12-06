using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class ServiceHandler
    {
        /**
         * Unklar: Authorization
         * Nimmt die Parameter vom Client (durch die Implementation von ServerModule.Http) entgegen
         * Erstellt eine Instanz von Requesthandler, übergibt die Daten, Rückgabewert wird als Response
         * wieder an den Client gesendet.
         **/
        //private string HttpMethod { get; set; }
        //private string RawUrl { get; set; }
        //private string Host { get; set; }
        //private string UserAgent { get; set; }
        //private string Accept { get; set; }
        //private string ContentType { get; set; }
        //private string ContentLength { get; set; }
        //private string Authorization { get; set; }
        private enum Chars
        {
            WhiteSpace, NewLine
        }
        private readonly Dictionary<Enum, char> _separators = new Dictionary<Enum, char>() { { Chars.WhiteSpace, ' ' }, { Chars.NewLine, '\n' } };

        public void Handle(string message)
        {
            List<string> httpRequestParts = new List<string>(message.Split(_separators[Chars.NewLine]));
            Dictionary<string, string> headers = new Dictionary<string, string>();
            Console.WriteLine(httpRequestParts.Count);
            // prüfen ob der Request genug Information enthält
            if (httpRequestParts.Count >= 3)
            {
                string[] metaData = httpRequestParts[0].Split(_separators[Chars.WhiteSpace]);
                string httpMethod = metaData[0];
                string rawUrl = metaData[1];
                string httpVersion = metaData[2];

                httpRequestParts.RemoveRange(0, 3);
            }
        }
    }
}
