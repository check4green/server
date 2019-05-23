using SensorsManager.Web.Api.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class SensorModelPut
    {

        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [Range(1, double.PositiveInfinity)]
        public int UploadInterval { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}