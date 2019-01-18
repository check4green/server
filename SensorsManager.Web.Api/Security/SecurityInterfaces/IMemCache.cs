using System;

namespace SensorsManager.Web.Api.Security
{
    public interface IMemCache
    {
        object Get(string email);
        void Remove(string email);
        void Add(string cacheKey, string item, DateTimeOffset dateTimeOffset);
    }
}
