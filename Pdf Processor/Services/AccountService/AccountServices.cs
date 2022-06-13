
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Cryptography;
using Pdf_Processor.Data;
using PdfSharpCore.Models;
using Pdf_Processor.Models;
using Pdf_Processor.ViewModels;
using static Pdf_Processor.Helper.MyExceptionResponse;
using Pdf_Processor.Helper;
using Pdf_Processor.Services.EmailService;
using System.Web.Helpers;

namespace Pdf_Processor.Services.AccountService
{
    public class AccountServices : IAccountServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpcontextAcessor;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<ApplicationRole> _rolemanager;
        
        public AccountServices(ApplicationDbContext context,
                               UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               IHttpContextAccessor httpContextAccessor,
                               IConfiguration configuration,
                               IEmailService emailSrv,
                               RoleManager<ApplicationRole> roleManager
            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpcontextAcessor = httpContextAccessor;
            _configuration = configuration;
            _rolemanager = roleManager;
            this._emailService = emailSrv;
        }

        public async Task<ResponseViewModel<object>> Signup(SignupViewModel user)
        {
            try
            {
                var appUser = new ApplicationUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.Email,
                    Active = true
                };
                var passwordValidator = new PasswordValidator<ApplicationUser>();
                var valid = await passwordValidator.ValidateAsync(_userManager, null, user.Password);
                if (!valid.Succeeded)
                    throw new HttpStatusException(System.Net.HttpStatusCode.UnprocessableEntity, "Passwords must have at least one uppercase ('A'-'Z'), one digit ('0'-'9') and one non alphanumeric character.");

                var result = await _userManager.CreateAsync(appUser, user.Password);

                if (!result.Succeeded)
                {
                    var userindb = await _userManager.FindByEmailAsync(user.Email);
                    if (userindb.EmailConfirmed == true)
                    {
                        return new ResponseViewModel<object>
                        {
                            Status = false,
                            Message = "Invalid Request, User with this email already exist.",
                            StatusCode = System.Net.HttpStatusCode.UnprocessableEntity.ToString(),
                        };
                        //throw new HttpStatusException(System.Net.HttpStatusCode.UnprocessableEntity, "Invalid Request, User with this email already exist.");
                    }
                    else
                    {
                        return new ResponseViewModel<object>
                        {
                            Status = false,
                            Message = result.Errors.FirstOrDefault().Description.ToString(),
                            StatusCode = System.Net.HttpStatusCode.UnprocessableEntity.ToString(),
                            Data = new { type = "USER_EXIST_EMAIL_NOT_VERIFIED" }
                        };
                    }
                }
                else
                {
                    var token = await this._userManager.GenerateEmailConfirmationTokenAsync(appUser);
                    await this._userManager.ConfirmEmailAsync(appUser, token);

                    await _userManager.AddToRoleAsync(appUser, "User");
                    
                    await _context.SaveChangesAsync();

                    
                    var email = new SendMailViewModel { Email = appUser.Email };

                    //await SendConfirmationLink(email);


                }
                return new ResponseViewModel<object>
                {
                    Status = true,
                    Message = "Signup Successfully, Please Confirm your Email",
                    StatusCode = System.Net.HttpStatusCode.OK.ToString(),
                    Data = appUser
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<ResponseViewModel<object>> LogIn(LoginViewModel data)
        {
            try
            {
                var userResult = await _userManager.FindByNameAsync(data.Email);

                if (userResult == null)
                {
                    throw new HttpStatusException(System.Net.HttpStatusCode.UnprocessableEntity, "Invalid Credentials");
                    //return new ResponseViewModel<object>
                    //{
                    //    Status = false,
                    //    Message = "Invalid Credentials",
                    //    StatusCode = System.Net.HttpStatusCode.BadRequest.ToString()
                    //};
                }

                bool emailStatus = await _userManager.IsEmailConfirmedAsync(userResult);
                if (!emailStatus)
                {
                    return new ResponseViewModel<object>
                    {
                        Status = false,
                        Message = "Email is not confirmed, Please confirm it & Try Again",
                        StatusCode = System.Net.HttpStatusCode.UnprocessableEntity.ToString(),
                        Data = userResult.Email
                    };
                }

                if (userResult.Active == false)
                    throw new HttpStatusException(System.Net.HttpStatusCode.UnprocessableEntity, "Your Account is Inactive. Please contact to the admin");


                var result = await _signInManager.PasswordSignInAsync(userResult, data.Password, false, false);
                var userRole = _userManager.GetRolesAsync(userResult).Result.FirstOrDefault();

                if (!result.Succeeded)
                    throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, "Invalid Credentials");
                //return new ResponseViewModel<object>
                //{
                //    Status = false,
                //    Message = "Invalid Credentials",
                //    StatusCode = System.Net.HttpStatusCode.BadRequest.ToString()
                //};

                var token = AuthenticateUser(data.Email, data.Password, userResult.Id, userRole);
                var tokres = await _userManager.SetAuthenticationTokenAsync(await _userManager.FindByNameAsync(data.Email), "JWT", "JWT Token", token);

                var loginRes = new LoginResponseViewModel
                {
                    UserId = userResult.Id,
                    Email = userResult.Email,
                    FirstName = userResult.FirstName,
                    LastName = userResult.LastName,
                    UserName = userResult.UserName,
                    Role = userRole,
                    JwtToken = token,
                    Expiry = "SESSION_WILL_EXPIRE_IN_1_DAY"
                };

                return new ResponseViewModel<object>
                {
                    Status = true,
                    Message = "Logged In Successfully",
                    StatusCode = System.Net.HttpStatusCode.OK.ToString(),
                    Data = loginRes
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseViewModel<object>> LogOut(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            await _userManager.RemoveAuthenticationTokenAsync(user, "JWT", "JWT Token");

            var userid = _context.Users.Where(s => s.UserName == user.UserName).Select(s => s.Id).FirstOrDefault();

            // create session for login user
            var bearertoken = _httpcontextAcessor.HttpContext.Request.Headers["Authorization"].ToString();
            var array = bearertoken.Split(" ");
            var token = array[1];

            return new ResponseViewModel<object>()
            {
                Status = true,
                Message = "User Logged Out",
                StatusCode = System.Net.HttpStatusCode.OK.ToString()
            };
        }

        public async Task<ResponseViewModel<object>> ChangePassword(ChangePasswordViewModel data)
        {
            try
            {
                var passwordValidator = new PasswordValidator<ApplicationUser>();
                var valid = await passwordValidator.ValidateAsync(_userManager, null, data.NewPassword);
                if (!valid.Succeeded)
                    throw new HttpStatusException(System.Net.HttpStatusCode.UnprocessableEntity, "Passwords must have at least one uppercase ('A'-'Z'), one digit ('0'-'9') and one non alphanumeric character.");

                var user = _context.Users.Where(s => s.Id == data.Id).FirstOrDefault();
                var loginuserid = Guid.Parse(_httpcontextAcessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (loginuserid == user.Id)
                {
                    var oldPwd = await passwordValidator.ValidateAsync(_userManager, null, data.OldPassword);
                    if (!oldPwd.Succeeded)
                        throw new HttpStatusException(System.Net.HttpStatusCode.UnprocessableEntity, "Passwords must have at least one uppercase ('A'-'Z'), one digit ('0'-'9') and one non alphanumeric character.");

                    var result = await _userManager.ChangePasswordAsync(user, data.OldPassword, data.NewPassword);
                    if (!result.Succeeded)
                        return new ResponseViewModel<object>
                        {
                            Status = false,
                            Message = "Incorrect Old Password",
                            StatusCode = System.Net.HttpStatusCode.UnprocessableEntity.ToString(),
                        };
                    else
                        return new ResponseViewModel<object>
                        {
                            Status = true,
                            Message = "Password Updated Succesfully",
                            StatusCode = System.Net.HttpStatusCode.OK.ToString(),
                        };
                }

                user.PasswordHash = Crypto.HashPassword(data.NewPassword);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return new ResponseViewModel<object>
                {
                    Status = true,
                    Message = "Password Updated Succesfully",
                    StatusCode = System.Net.HttpStatusCode.OK.ToString(),
                };

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseViewModel<object>> ChangeRole(Guid userid, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userid.ToString());
                if (user == null)
                    throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, "Specified user does not exist");

                var roleexist = await _rolemanager.FindByNameAsync(role);
                if (roleexist == null)
                    throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, "Specified role does not exist");

                //removing from all roles and then adding to specified role
                var removeResult = await _userManager.RemoveFromRolesAsync(user, new List<string> { "User" });
                var removeResult1 = await _userManager.RemoveFromRolesAsync(user, new List<string> { "Admin" });
                var roleResult = await _userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                    return new ResponseViewModel<object>
                    {
                        Status = false,
                        Message = roleResult.Errors.FirstOrDefault().Description,
                        StatusCode = System.Net.HttpStatusCode.BadRequest.ToString(),
                    };

                return new ResponseViewModel<object>
                {
                    Status = true,
                    Message = "Admin Role Assigned",
                    StatusCode = System.Net.HttpStatusCode.OK.ToString(),
                };

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseViewModel<object>> Update(UsersViewModel data)
        {
            try
            {
                var user = _context.Users.Where(s => s.Id == data.Id).FirstOrDefault();
                

                user.FirstName = data.FirstName;
                user.LastName = data.LastName;


                _context.Users.Update(user);
                await this._context.SaveChangesAsync();


                return new ResponseViewModel<object>
                {
                    Status = true,
                    Message = "User Updated",
                    StatusCode = System.Net.HttpStatusCode.OK.ToString(),
                    Data = user
                };

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseViewModel<object>> GetAllUsers()
        {
            try
            {
                var loginuserid = Guid.Parse(_httpcontextAcessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                var users = await _context.Users.Where(s => s.Id != loginuserid).ToListAsync();

                foreach (var user in users)
                {
                    var userRole = await this._context.UserRoles.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
                    if (userRole != null)
                    {
                        var role = await this._context.Roles.Where(x => x.Id == userRole.RoleId).FirstAsync();
                        user.Role = role;
                    }
                }


                return new ResponseViewModel<object>
                {
                    Status = true,
                    Message = "User Updated",
                    StatusCode = System.Net.HttpStatusCode.OK.ToString(),
                    Data = users.Select(x => x.toUserViewModel())
                };

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseViewModel<object>> SendConfirmationLink(SendMailViewModel data)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(data.Email);
                if (user == null)
                    throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, "Email doesn't exist, User not found.");
                var token = HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(user));
                var link = _configuration.GetValue<string>("BaseUrl:API_Url");
                var confirmationlink = link + "api/Account/ConfirmEmailLink?token=" + token + "&email=" + user.Email;

                //var emailtemplate = new EmailTemplate();
                //emailtemplate.Link = confirmationlink;

                Dictionary<string, string> emaildata = new();
                emaildata.Add("firstname", user.FirstName);
                emaildata.Add("To", user.Email);
                emaildata.Add("Link", confirmationlink);
                //emaildata.Add("From", "support@mhparks.com");
                var dynamicEmailsent = _emailService.SendSmtpEmailDynamic("EmailConfirmationLink", emaildata);
                //emailtemplate.UserId = user.Id;
                //emailtemplate.EmailType = EmailType.EmailConfirmationLink;
                //var emailsent = _emailService.SendSmtpMail(emailtemplate);
                if (dynamicEmailsent != true)
                    throw new HttpStatusException(System.Net.HttpStatusCode.InternalServerError, "Email not sent.");


                return new ResponseViewModel<object>
                {
                    Status = true,
                    Message = "Link Sent Successfully",
                    StatusCode = System.Net.HttpStatusCode.OK.ToString(),
                    Data = confirmationlink
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string AuthenticateUser(string username, string password, Guid userid, string role)
        {

            var _key = ProjectConstants.AuthKey;
            var key = Encoding.UTF8.GetBytes(_key);
            var tokenhanlder = new JwtSecurityTokenHandler();

            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, userid.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenhanlder.CreateToken(tokendescriptor);
            var token = tokenhanlder.WriteToken(stoken);
            return token;
        }


    }

}

