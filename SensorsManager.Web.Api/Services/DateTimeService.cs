using SensorsManager.Web.Api.ServiceInterfaces;
using System;

namespace SensorsManager.Web.Api.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTimeOffset GetDateOffSet()
        {
            return DateTimeOffset.UtcNow;
        }

        public DateTime GetDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}