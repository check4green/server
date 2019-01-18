using System;

namespace SensorsManager.Web.Api.Models
{
    public class SensorReadingModelGet
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset ReadingDate { get; set; }
    }
}