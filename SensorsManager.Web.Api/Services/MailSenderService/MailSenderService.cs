using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace SensorsManager.Web.Api.Services
{
    public class MailSenderService : IMailSenderService
    {
        private static readonly string _email = ConfigurationManager.AppSettings["mailservice:email"];
        private static readonly string _password = ConfigurationManager.AppSettings["mailservice:password"];
        private static readonly string _host = ConfigurationManager.AppSettings["mailservice:host"];
        private static readonly int _port = int.Parse(ConfigurationManager.AppSettings["mailservice:port"]);
        public void SendMail(string receiver, string subject = "", string body = "")
        {
            var mailMessage = new MailMessage(_email, receiver)
            {
                Subject = subject,
                Body = body
            };

            var smptClient = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential()
                {
                    UserName = _email,
                    Password = _password
                },
                EnableSsl = false,
            };
            smptClient.Send(mailMessage);
        }
    }
}