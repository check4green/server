using System;

namespace SensorsManager.Web.Api.Models
{
    public class SensorReadingModelPostAddres
    {
        public string SensorGatewayAddress { get; set; }
        public string SensorClientAddress { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset ReadingDate { get; set; }
    }
}