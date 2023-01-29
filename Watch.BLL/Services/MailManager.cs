using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Watch.DAL.Data;

namespace Watch.BLL.Services
{
    public class MailManager : IMailService
    {
        private readonly MailSettings _mailsettings;

        public MailManager(IOptions<MailSettings> mailsettings)
        {
            _mailsettings = mailsettings.Value;
        }

        public async Task SendEmailAsync(RequestEmail mailRequest)
        {
            try
            {
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_mailsettings.Mail)
                };

                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder
                {
                    HtmlBody = mailRequest.Body
                };
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();

                smtp.Connect(_mailsettings.Host, _mailsettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailsettings.Mail, _mailsettings.Password);

                await smtp.SendAsync(email);

                smtp.Disconnect(true);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
    }
}
