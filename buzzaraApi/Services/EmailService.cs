using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace buzzaraApi.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarEmailAsync(string para, string assunto, string corpoHtml)
        {
            var smtpConfig = _configuration.GetSection("Smtp");

            var host = smtpConfig["Host"] ?? throw new ArgumentNullException("Host");
            var port = smtpConfig["Port"] ?? throw new ArgumentNullException("Port");
            var username = smtpConfig["Username"] ?? throw new ArgumentNullException("Username");
            var password = smtpConfig["Password"] ?? throw new ArgumentNullException("Password");
            var sender = smtpConfig["Sender"] ?? throw new ArgumentNullException("Sender");

            using var client = new SmtpClient
            {
                Host = host,
                Port = int.Parse(port),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            var mensagem = new MailMessage
            {
                From = new MailAddress(sender),
                Subject = assunto,
                Body = corpoHtml,
                IsBodyHtml = true
            };

            mensagem.To.Add(para);

            await client.SendMailAsync(mensagem);
        }
    }
}
