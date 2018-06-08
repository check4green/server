using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorReadingModel2
    {
        [Required]
        public string SensorGatewayAddress { get; set; }
        [Required]
        public string SensorClientAddress { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        public DateTimeOffset ReadingDate { get; set; }


    }
}