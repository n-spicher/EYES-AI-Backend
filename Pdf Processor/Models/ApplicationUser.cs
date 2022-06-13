
using Microsoft.AspNetCore.Identity;
using Pdf_Processor.Models;
using Pdf_Processor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfSharpCore.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }

        [NotMapped]
        public ApplicationRole Role { get; set; }
    }

    public static class ApplicationUserExtension
    {
        public static UsersViewModel toUserViewModel(this ApplicationUser obj)
        {
            if (obj == null) return null;

            return new UsersViewModel
            {
                Id = obj.Id,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Email = obj.Email,
                Active = obj.Active,
                Role = obj.Role?.Name
            };
        }
    }
}
