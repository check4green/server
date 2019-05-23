namespace SensorsManager.Web.Api.Services
{
    public interface IThrottlerService
    {
        int RequestLimit { get; }
        int TimeoutInSeconds { get; }

        void ThrottlerSetup(string key, int requestLimit = 5, int timeoutInSeconds = 10);
        bool RequestShouldBeThrottled();
    }
}
