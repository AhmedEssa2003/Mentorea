using MailKit.Net.Smtp;
using MailKit.Security;
using Mentorea.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Mentorea.Services
{
    public class MailService(IOptions<MailSettings> mailSettings) : IEmailSender
    {
        private readonly MailSettings _mailSettings = mailSettings.Value;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var Message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                Subject = subject
            };

            Message.To.Add(MailboxAddress.Parse(email));
            Message.ReplyTo.Add(MailboxAddress.Parse(_mailSettings.Mail));
            var Builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            Message.Body = Builder.ToMessageBody();
            using var Smtp = new SmtpClient();

            Smtp.Connect(_mailSettings.Host, _mailSettings.Port,SecureSocketOptions.StartTls);
            Smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await Smtp.SendAsync(Message);
            Smtp.Disconnect(true);
        }
    }
}
