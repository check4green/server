namespace SensorsManager.Web.Api.Services
{
    public interface IMailSenderService
    {
        void SendMail(string receiver, string subject = "", string body = "");
    }
}
