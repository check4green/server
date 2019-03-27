using System;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class SensorReadingModelPost
    {
        [Required]
        public string SensorGatewayAddress { get; set; }
        [Required]
        public string SensorClientAddress { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset ReadingDate { get; set; }
    }
}