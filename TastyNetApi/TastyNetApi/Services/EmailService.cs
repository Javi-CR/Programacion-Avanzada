using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace TastyNetApi.Services
{
    public class EmailService
    {
        private readonly string _apiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            // Configuración de SendGrid desde appsettings.json
            var sendGridConfig = configuration.GetSection("SendGrid");
            _apiKey = sendGridConfig["ApiKey"] ?? throw new ArgumentNullException("ApiKey", "La clave API de SendGrid no está configurada.");
            _senderEmail = sendGridConfig["SenderEmail"] ?? throw new ArgumentNullException("SenderEmail", "El correo electrónico del remitente no está configurado.");
            _senderName = sendGridConfig["SenderName"] ?? "Default Sender";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_senderEmail, _senderName);
            var to = new EmailAddress(toEmail);
            var email = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            var response = await client.SendEmailAsync(email);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al enviar el correo: {response.StatusCode}");
            }
        }
    }
}