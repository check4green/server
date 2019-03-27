using SensorsManager.Web.Api.ServiceInterfaces;
using System;
using System.Collections.Concurrent;

namespace SensorsManager.Web.Api.Security.ServiceInterfaces
{
    public class ThrottlerService : IThrottlerService
    {
        public int RequestLimit { get; private set; }
        public int RequestsRemaining { get; private set; }
        public int ExpiresAt
        {
            get
            {
                return (_cache[_key].ExpiresAt - DateTime.UtcNow).Seconds;
            }
        }
        public DateTime WindowResetDate { get; private set; }
        public int TimeoutInSeconds { get; private set; }
        

        private static ConcurrentDictionary<string, ThrottleInfo> _cache =
            new ConcurrentDictionary<string, ThrottleInfo>();

        private string _key;

        public void ThrottlerSetup(string key, int requestLimit = 5, int timeoutInSeconds = 10)
        {
            RequestLimit = requestLimit;
            TimeoutInSeconds = timeoutInSeconds;
            _key = key;
        }

        public bool RequestShouldBeThrottled()
        {
            ThrottleInfo throttleInfo = _cache.ContainsKey(_key) ? _cache[_key] : null;

            if (throttleInfo == null || throttleInfo.ExpiresAt <= DateTime.Now)
            {
                throttleInfo = new ThrottleInfo
                {
                    ExpiresAt = DateTime.Now.AddSeconds(TimeoutInSeconds),
                    RequestCount = 0
                };
            };

            WindowResetDate = throttleInfo.ExpiresAt;

            throttleInfo.RequestCount++;

            _cache[_key] = throttleInfo;

            RequestsRemaining = Math.Max(RequestLimit - throttleInfo.RequestCount, 0);

            return (throttleInfo.RequestCount > RequestLimit);
        }

        private class ThrottleInfo
        {
            public DateTime ExpiresAt { get; set; }
            public int RequestCount { get; set; }
        }
    }
}