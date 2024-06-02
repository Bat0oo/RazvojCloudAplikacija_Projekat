
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NotificationService
{
    public class EmailSender
    {
        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("drs.productmanagement@gmail.com", "cwnu vqzv orrj lyty"),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("drs.productmanagement@gmail.com"),
                    Subject = subject,
                    Body = body,
                };
                mailMessage.To.Add(recipient);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Mejl uspešno poslat.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom slanja mejla: {ex.Message}");
            }
        }
    }
}
