namespace SensorsManager.Web.Api.Security
{
    public interface IMailSender
    {
        void SendMail(string receiver, string subject = "", string body = "");
    }
}
