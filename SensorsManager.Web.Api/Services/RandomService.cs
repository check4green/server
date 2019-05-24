using System;

namespace SensorsManager.Web.Api.Services
{
    public class RandomService : Random, IRandomService
    {
        public int GetRandomNumber(int minValue, int maxValue)
        {
            var randomNumber = Next(1000, 9999);
            return randomNumber;
        }
    }
}