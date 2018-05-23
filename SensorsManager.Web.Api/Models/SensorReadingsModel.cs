using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Models
{
    public class SensorReadingModel
    {
        public int Id { get; set; }
        public string SensorGatewayAdress { get; set; }
        public string SensorClientAdress { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset ReadingDate { get; set; }
    }
}