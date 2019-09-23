using System;

namespace SensorsManager.Web.Api.Services
{
    public interface IDateTimeService
    {
        DateTime GetDateTime();
        DateTimeOffset GetDateOffSet();
    }
}
