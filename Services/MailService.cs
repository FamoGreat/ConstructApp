using ConstructApp.Models;
using ConstructApp.Settings;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace ConstructApp.Services
{
    public class MailService : IMailService, IEmailSender
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to send email", ex);
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailRequest = new MailRequest
            {
                ToEmail = email,
                Subject = subject,
                Body = htmlMessage
            };
            await SendEmailAsync(mailRequest);
        }
    }
}
