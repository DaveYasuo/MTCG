using System.Collections.Generic;

namespace MTCG.Logging
{
    public class CardLog
    {
        public string Base { get; set; }
        public List<string> Description { get; set; } = new();
        public string Effective { get; set; }
    }
}