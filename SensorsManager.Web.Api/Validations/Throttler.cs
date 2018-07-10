using System;
using System.Collections.Concurrent;

namespace SensorsManager.Web.Api.Validations
{
    public class Throttler
    {
        private int _requestLimit;
        private int _timeoutInSeconds;
        private string _key;
        private static ConcurrentDictionary<string, ThrottleInfo> _cache =
            new ConcurrentDictionary<string, ThrottleInfo>();

        public Throttler(string key, int requestLimit = 1, int timeoutInSeconds = 3)
        {
            _requestLimit = requestLimit;
            _timeoutInSeconds = timeoutInSeconds;
            _key = key;
        }

        public bool RequestShouldBeThrottled()
        {
            ThrottleInfo throttleInfo = _cache.ContainsKey(_key) ? _cache[_key] : null;

            if (throttleInfo == null || throttleInfo.ExpiresAt <= DateTime.Now)
            {
                throttleInfo = new ThrottleInfo
                {
                    ExpiresAt = DateTime.Now.AddSeconds(_timeoutInSeconds),
                    RequestCount = 0
                };
            };

            throttleInfo.RequestCount++;

            _cache[_key] = throttleInfo;

            return (throttleInfo.RequestCount > _requestLimit);
        }



        private class ThrottleInfo
        {
            public DateTime ExpiresAt { get; set; }
            public int RequestCount { get; set; }
        }

    }
}