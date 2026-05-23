using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Project.Options;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
namespace Project.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpOptions _smtpOptions;

        public EmailSender(IOptions<SmtpOptions> smtpOptions)
        {

          _smtpOptions = smtpOptions.Value;

        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            try
            {
                var body = new TextPart(MimeKit.Text.TextFormat.Html);
                body.Text = htmlMessage;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
                message.Subject= subject;
                message.To.Add(new MailboxAddress(null, email));
                message.Body= body;

                var client = new SmtpClient();
                await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
                await client.AuthenticateAsync(_smtpOptions.SenderEmail,_smtpOptions.Password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);


            


            }
            catch (Exception)
            {

               
            }
        }
    }
}
