using System;
using System.Runtime.Caching;

namespace SensorsManager.Web.Api.Security
{
    public class MemCache : IMemCache
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