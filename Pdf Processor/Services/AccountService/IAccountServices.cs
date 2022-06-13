
using Microsoft.AspNetCore.Mvc;
using Pdf_Processor.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pdf_Processor.Services.AccountService
{
    public interface IAccountServices
    {
        public Task<ResponseViewModel<object>> Signup(SignupViewModel user);
        public Task<ResponseViewModel<object>> LogIn(LoginViewModel data);
        public Task<ResponseViewModel<object>> LogOut(string username);

        public Task<ResponseViewModel<object>> ChangePassword(ChangePasswordViewModel data);
        public Task<ResponseViewModel<object>> Update(UsersViewModel data);

        public Task<ResponseViewModel<object>> GetAllUsers();

        public Task<ResponseViewModel<object>> ChangeRole(Guid userid, string role);


    }
}
