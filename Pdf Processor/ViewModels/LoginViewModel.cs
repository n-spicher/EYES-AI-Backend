using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf_Processor.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(62)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
