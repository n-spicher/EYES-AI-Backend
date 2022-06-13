
using System.Collections.Generic;

namespace Pdf_Processor.Services.EmailService
{
    public interface IEmailService
    {
        public bool SendSmtpEmailDynamic(string emailType, IDictionary<string, string> data, string filePath = "", string contentType = "");
    }
}