using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf_Processor.ViewModels
{
    public class ChangePasswordViewModel
    {
        public Guid Id { get; set; }
        [MinLength(8)]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
        [Required]
        [MinLength(8)]
        public string ConfirmPassword { get; set; }
    }
}
