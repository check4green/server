namespace SensorsManager.Web.Api.ServiceInterfaces
{
    public interface IMailSenderService
    {
        void SendMail(string receiver, string subject = "", string body = "");
    }
}
