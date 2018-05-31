using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorReadingModel3
    {
        public int SensorId { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset ReadingDate { get; set; }
    }
}