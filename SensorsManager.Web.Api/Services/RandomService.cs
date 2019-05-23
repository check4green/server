using System;

namespace SensorsManager.Web.Api.Services
{
    public class RandomService : Random, IRandomService
    {
        public override int Next(int minValue, int maxValue)
        {
            return Next(minValue, maxValue);
        }
    }
}