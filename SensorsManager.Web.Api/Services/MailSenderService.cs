

using SensorsManager.Web.Api.Security.ServiceInterfaces;
using SensorsManager.Web.Api.ServiceInterfaces;
using System.Net;
using System.Net.Mail;

namespace SensorsManager.Web.Api.Services
{
    public class MailSenderService : IMailSenderService
    {
        public void SendMail(string receiver, string subject = "", string body = "")
        {
            var data = new { Email = "info@check4green.com", Password = "C6theo^AntU[" };

            var mailMessage = new MailMessage(data.Email, receiver)
            {
                Subject = subject,
                Body = body
            };

            var smptClient = new SmtpClient("mail.check4green.com", 587)
            {
                Credentials = new NetworkCredential()
                {
                    UserName = data.Email,
                    Password = data.Password
                },
                EnableSsl = false,
            };
            smptClient.Send(mailMessage);
        }
    }
}