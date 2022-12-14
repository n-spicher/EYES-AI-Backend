using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Models
{
    public class FoundKeywords
    {
        public string sectionId { get; set; }
        public string sectionName { get; set; }
        public Dictionary<string, List<string>> matches { get; set;}

    }
}