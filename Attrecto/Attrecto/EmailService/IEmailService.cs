using Attrecto.Data;
using System.Net;

namespace Attrecto.EmailService
{
    public interface IEmailService
    {
        Task<HttpStatusCode> SendEmailAsync(string email, string subject, string description);
    }
}
