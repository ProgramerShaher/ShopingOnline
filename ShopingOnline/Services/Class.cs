using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace ShopingOnline.Services
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"[FakeEmailSender] Email sent to: {email}\nSubject: {subject}\nBody: {htmlMessage}");
            return Task.CompletedTask;
        }
    }
}
