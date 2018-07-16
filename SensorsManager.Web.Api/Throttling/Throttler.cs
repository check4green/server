using System;
using System.Collections.Concurrent;

namespace SensorsManager.Web.Api.Throttling
{
    public class Throttler
    {
        private int _requestLimit;
        private int _timeoutInSeconds;
       
        private static ConcurrentDictionary<string, ThrottleInfo> _cache =
            new ConcurrentDictionary<string, ThrottleInfo>();

        public string ThrottleGroup { get; set; }
        public int RequestLimit { get => _requestLimit; }
        public int RequestCount { get => _cache[ThrottleGroup].RequestCount; }
        public int ExpiresAt { get
            {
                return (_cache[ThrottleGroup].ExpiresAt - DateTime.UtcNow).Seconds;
            } }
       

        public Throttler(string key, int requestLimit = 1, int timeoutInSeconds = 2)
        {
            _requestLimit = requestLimit;
            _timeoutInSeconds = timeoutInSeconds;
            ThrottleGroup  = key;
        }

        public bool RequestShouldBeThrottled()
        {
            ThrottleInfo throttleInfo = _cache.ContainsKey(ThrottleGroup ) ? _cache[ThrottleGroup ] : null;

            if (throttleInfo == null || throttleInfo.ExpiresAt <= DateTime.Now)
            {
                throttleInfo = new ThrottleInfo
                {
                    ExpiresAt = DateTime.Now.AddSeconds(_timeoutInSeconds),
                    RequestCount = 0
                };
            };


            if (!(throttleInfo.RequestCount >= _requestLimit))
            {
                throttleInfo.RequestCount++;
            }
     
            _cache[ThrottleGroup] = throttleInfo;

            return (throttleInfo.RequestCount > _requestLimit);
        }



        private class ThrottleInfo
        {
            public DateTime ExpiresAt { get; set; }
            public int RequestCount { get; set; }
        }

    }
}