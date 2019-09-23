using System;

namespace SensorsManager.Web.Api.Services
{
    public interface IMemCacheService
    {
        object Get(string email);
        void Remove(string email);
        void Add(string cacheKey, string item, DateTimeOffset dateTimeOffset);
    }
}
