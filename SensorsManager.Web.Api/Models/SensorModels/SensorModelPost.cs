using SensorsManager.Web.Api.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class SensorModelPost
    {

        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int SensorTypeId { get; set; }
        public int NetworkId { get; set; }
        public DateTime ProductionDate { get; set; }
        [Required]
        [Range(1, 262975)]
        public int UploadInterval { get; set; }
        [Required, StringLength(10)]
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}