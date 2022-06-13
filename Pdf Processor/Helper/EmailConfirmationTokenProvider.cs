using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Helper
{
    public class EmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<EmailConfirmationTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }
    }
    public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public EmailConfirmationTokenProviderOptions()
        {
            Name = "EmailDataProtectorTokenProvider";
            TokenLifespan = TimeSpan.FromDays(1);
        }
    }

    public class ResetPasswordTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public ResetPasswordTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<ResetPasswordTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }
    }
    public class ResetPasswordTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public ResetPasswordTokenProviderOptions()
        {
            Name = "ResetPasswordTokenProvider";
            TokenLifespan = TimeSpan.FromDays(1);
        }
    }
}