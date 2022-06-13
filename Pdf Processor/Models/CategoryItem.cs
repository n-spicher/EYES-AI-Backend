using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int CategoryId { get; set; }

        public ICollection<CategoryItemSynonym> CategoryItemSynonyms { get; set; }
    }
}
