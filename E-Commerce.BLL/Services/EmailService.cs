// Services/IEmailService.cs
using E_Commerce.DAL.DB_Context;
using E_Commerce.DAL.Entity_Extension;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Text;
using YourApp.Services;

namespace YourApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            var subject = "Confirm your email";
            var htmlMessage = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>Welcome to YourApp!</h2>
                    <p>Please confirm your account by clicking the link below:</p>
                    <p>
                        <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>
                            Confirm Email
                        </a>
                    </p>
                    <p>If you didn't create an account, please ignore this email.</p>
                    <p>Thanks,<br>The YourApp Team</p>
                </div>";

            await SendEmailAsync(email, subject, htmlMessage);
        }

        public async Task SendPasswordResetAsync(string email, string resetLink)
        {
            var subject = "Reset your password";
            var htmlMessage = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>Password Reset Request</h2>
                    <p>You requested a password reset. Click the link below to reset your password:</p>
                    <p>
                        <a href='{resetLink}' style='display: inline-block; padding: 10px 20px; background-color: #dc3545; color: white; text-decoration: none; border-radius: 5px;'>
                            Reset Password
                        </a>
                    </p>
                    <p>If you didn't request a password reset, please ignore this email.</p>
                    <p>This link will expire in 24 hours for security reasons.</p>
                    <p>Thanks,<br>The YourApp Team</p>
                </div>";

            //await SendEmailAsync(email, subject, htmlMessage);
            using(var write = new StreamWriter(@"C:\Users\abdelrahman.abdalla\Desktop\Test\test6.text"))
            {
                write.WriteLine("{0},{1},{2}", email, subject, htmlMessage);
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port);
                client.EnableSsl = _emailSettings.UseSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", email);
                throw;
            }
        }
    }
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email, string confirmationLink);
        Task SendPasswordResetAsync(string email, string resetLink);
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
    // Configuration class for email settings
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}

// Program.cs Configuration
// Add this to your Program.cs file