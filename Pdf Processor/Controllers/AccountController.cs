using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pdf_Processor.Data;
using Pdf_Processor.Models;
using Pdf_Processor.Services.AccountService;
using Pdf_Processor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAccountServices _accountServices;

        public AccountController(
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context,
            IAccountServices accountServices
            )
        {
            this._roleManager = roleManager;
            this._context = context;
            this._accountServices = accountServices;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<object> SeedRoles()
        {
            List<ApplicationRole> Roles = new List<ApplicationRole>
                {
                    new ApplicationRole
                    {
                        Name="SuperAdmin"
                    },
                    new ApplicationRole
                    {
                        Name="Admin"
                    },
                    new ApplicationRole
                    {
                        Name="User"
                    },
                };

            foreach (var role in Roles)
            {
                var doexist = await _roleManager.RoleExistsAsync(role.Name);
                if (!doexist)
                {
                    var roleResult = await _roleManager.CreateAsync(role);
                }
            }

            await this._context.SaveChangesAsync();

            var user = new SignupViewModel
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@gmail.com",
                Password = "123Admin@!",
                ConfirmPassword = "123Admin@!"
            };

            var oldAdmin = await this._context.Users.Where(x => x.Email == user.Email).FirstOrDefaultAsync();

            if (oldAdmin == null)
            {
                await this._accountServices.Signup(user);

                var admin = await this._context.Users.Where(x => x.Email == user.Email).FirstOrDefaultAsync();

                if (admin != null)
                {
                    await this._accountServices.ChangeRole(admin.Id, "SuperAdmin");
                }
            }

            

            return Roles;
        }

        [AllowAnonymous]
        [HttpPost("Signup")]
        public async Task<ResponseViewModel<object>> Signup(SignupViewModel user)
        {
            return await _accountServices.Signup(user);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ResponseViewModel<object>> LogIn(LoginViewModel data)
        {
            return await _accountServices.LogIn(data);
        }

        [HttpPost("ChangePassword")]
        public async Task<ResponseViewModel<object>> ChangePassword(ChangePasswordViewModel data)
        {
            return await _accountServices.ChangePassword(data);
        }

        [HttpPost("[action]")]
        public async Task<ResponseViewModel<object>> Update(UsersViewModel data)
        {
            return await _accountServices.Update(data);
        }

        [HttpGet("[action]")]
        public async Task<ResponseViewModel<object>> GetAllUsers()
        {
            return await _accountServices.GetAllUsers();
        }

        [HttpGet("[action]/{userid}")]
        public async Task<ResponseViewModel<object>> ChangeRole(Guid userid, [FromQuery] string role)
        {
            return await _accountServices.ChangeRole(userid, role);
        }

        [HttpGet("logout")]
        public async Task<ResponseViewModel<object>> LogOut()
        {
            return await _accountServices.LogOut(User.Identity.Name);
        }
    }
}
