using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.ViewModels
{
    public class CategoryItemViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Note { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public List<string> CategoryItemSynonyms { get; set; }
    }
}
