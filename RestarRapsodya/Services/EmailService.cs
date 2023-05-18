using RestarRapsodya.Models;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace RestarRapsodya.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuracion;

        public EmailService(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }
        public void SendEmail(EmailDTO solicitud)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuracion.GetSection("Email:UserName").Value));
            email.To.Add(MailboxAddress.Parse(solicitud.Para));
            email.Subject = solicitud.Asunto;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = solicitud.Contenido
            };
            using var smtp = new SmtpClient();
            smtp.Connect(
                _configuracion.GetSection("Email:Host").Value,
                Convert.ToInt32(_configuracion.GetSection("Email:Port").Value),
                SecureSocketOptions.StartTls);

            smtp.Authenticate(_configuracion.GetSection("Email:UserName").Value
                , _configuracion.GetSection("Email:PassWord").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
