using System;
using System.Runtime.Caching;

namespace SensorsManager.Web.Api.Services
{
    public class MemCacheService : IMemCacheService
    {
        public void Add(string cacheKey, string item, DateTimeOffset dateTimeOffset)
        {
            MemoryCache.Default.Add(cacheKey, item, dateTimeOffset);
        }

        public object Get(string key)
        {
            return MemoryCache.Default.Get(key);
        }

        public void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }
    }
}