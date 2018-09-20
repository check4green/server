using System;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class SensorReadingModelPostId
    {
        [Required]
        public int SensorId { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        public DateTimeOffset ReadingDate { get; set; }
    }
}