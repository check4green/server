using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorReadingModel3
    {
        [Required]
        public int SensorId { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        [Range(typeof(DateTime), "1/2/2018", "1/2/9999", ErrorMessage = "Value for {0} must be greater than {1}")]
        public DateTimeOffset ReadingDate { get; set; }
    }
}