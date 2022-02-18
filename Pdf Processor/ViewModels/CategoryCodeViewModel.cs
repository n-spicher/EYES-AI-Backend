using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.ViewModels
{
    public class CategoryCodeViewModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
