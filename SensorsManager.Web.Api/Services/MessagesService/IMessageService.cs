namespace SensorsManager.Web.Api.Services
{
    public interface IMessageService
    {
        string GetMessage(Custom customMessageType, string entityName);
        string GetMessage(Custom customMessageType, string entityName, string parameterName);
        string GetMessage(Generic genericMessageType);
        string GetMessage(Email emaiMessageType);
    }
}