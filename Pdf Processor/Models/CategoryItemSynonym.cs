using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Models
{
    public class CategoryItemSynonym
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryItemId { get; set; }
    }
}
