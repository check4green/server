using System;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class SensorReadingModelPost
    {
        [Required]
        public string GatewayAddress { get; set; }
        [Required]
        public string SensorAddress { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset ReadingDate { get; set; }
    }
}