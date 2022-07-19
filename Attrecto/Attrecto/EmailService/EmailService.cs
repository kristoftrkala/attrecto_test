using Attrecto.Data;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace Attrecto.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HttpStatusCode> SendEmailAsync(string email, string subject, string description)
        {
            var client = new SendGridClient(_configuration["Keys:SendGrid"]);
            var myMsg = new SendGridMessage()
            {
                //did only a single sender verification in sendgrid
                From = new EmailAddress("tofika93@gmail.com"),
                Subject = subject,
                Contents = new List<Content> {
                    new Content(MimeType.Text, description)
                }
            };

            myMsg.AddTo(new EmailAddress(email));

            var result = await client.SendEmailAsync(myMsg);
            return result.StatusCode;
        }
    }
}
