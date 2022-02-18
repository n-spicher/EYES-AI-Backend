using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CategoryItem> CategoryItems { get; set; }
        public ICollection<CategoryCode> CategoryCodes { get; set; }
    }
}
