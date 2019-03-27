using System;

namespace SensorsManager.Web.Api.ServiceInterfaces
{
    public interface IDateTimeService
    {
        DateTime GetDateTime();
        DateTimeOffset GetDateOffSet();
    }
}
