using System.Net;
using System.Net.Mail;

namespace SensorsManager.Web.Api.Security
{
    public class MailSender
    {
        public void SendMail(string receiver, string subject = "", string body = "")
        {
            var data = new { Email = "wearegreenlegion@gmail.com", Password = "weAreGreenLegion4" };

            var mailMessage = new MailMessage(data.Email, receiver)
            {
                Subject = subject,
                Body = body
            };

            var smptClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential()
                {
                    UserName = data.Email,
                    Password = data.Password
                },
                EnableSsl = true
            };
            smptClient.Send(mailMessage);
        }
    }
}