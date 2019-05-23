using System;

namespace SensorsManager.Web.Api.Services
{
    public class GuidService : IGuidService
    {
        public string GetAddress()
        {
            var address = GetGuid()
                .ToString()
                .Replace("-", "")
                .Substring(0, 10);

            return address;
        }

        public Guid GetGuid()
        {
            return Guid.NewGuid();
        }
    }
}