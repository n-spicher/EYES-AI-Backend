using Microsoft.EntityFrameworkCore;
using Pdf_Processor.Data;
using Pdf_Processor.Helper;
using Pdf_Processor.Models;
using PdfSharpCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pdf_Processor.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        public EmailService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public bool SendSmtpEmailDynamic(string emailType, IDictionary<string, string> data, string filePath = "", string contentType = "")
        {
            var email = new EmailTemplate();

            
            string body = email.Body;
            string subject = "";
            string from = null;

            subject = email.Subject;
            if (data.ContainsKey("Subject"))
            {
                subject = data["Subject"];
            }
            if (data.ContainsKey("To"))
            {
                email.To = data["To"];
            }

            if (data.ContainsKey("From"))
            {
                from = data["From"];
            }

            foreach (var key in data.Keys)
            {
                if (key.Length > 0 && key[0] == '{' && key[key.Length - 1] == '}')
                {
                    body = body.Replace(key, data[key]);
                    subject = subject.Replace(key, data[key]);
                }
                body = body.Replace("{" + key + "}", data[key]);
                subject = subject.Replace("{" + key + "}", data[key]);
            }

            email.Subject = subject;
            email.Body = body;

            MailMessage message = getMailMessage(email, from);

            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(contentType) && File.Exists(filePath))
            {
                var content = new ContentType(contentType);
                content.Name = Path.GetFileName(filePath);
                message.Attachments.Add(new Attachment(filePath));
            }

            

            var credentials = new NetworkCredential(ProjectConstants.smtpEmail, ProjectConstants.smtpPassword);
            // Smtp client
            var client = new SmtpClient()
            {
                Port = ProjectConstants.smtpPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = ProjectConstants.smptpHost,
                EnableSsl = true,
                Credentials = credentials
            };

            client.Send(message);

            if (message.Attachments != null)
            {
                for (Int32 i = message.Attachments.Count - 1; i >= 0; i--)
                {
                    message.Attachments[i].Dispose();
                }
                message.Attachments.Clear();
                message.Attachments.Dispose();
            }
            message.Dispose();
            message = null;

            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception)
            {
            }

            return true;
        }

        private string emailTemplateParsing(ApplicationUser? user, EmailTemplate email, string link,string link2="")
        {
            var body = email.Body;
            body = body.Replace("{firstname}", user.FirstName);

            if (body.Contains("{linkyes}"))
            {
                body = body.Replace("{linkyes}", link);
                body = body.Replace("{linkno}", link2);
            }
            else
                body = body.Replace("{Link}", link);
            
            return body;
        }
        

        private string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }
        private static MailMessage getMailMessage(EmailTemplate email, string from = null)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.Body = email.Body;
            mail.Subject = email.Subject;
            mail.From = new MailAddress(!string.IsNullOrEmpty(from) ? from : ProjectConstants.mailFrom);
            mail.To.Add(email.To);
            return mail;
        }
    }
}