using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebUygulamaProje1.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // buraya email gonderme islemlerini ekleyebiliriz
            return Task.CompletedTask;
        }
    }
}
