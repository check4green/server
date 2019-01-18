using SensorsManager.Web.Api.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class SensorModelPut
    {

        [Required, MaxLength(50)]
        [RegularExpression("^[a-zA-Z0-9_-]*$", 
            ErrorMessage = "Only alphabets, numbers and the simbols: - or _ are allowed."),
            CustomValidation(typeof(SensorValidation), "NameValidation")]
        public string Name { get; set; }
        [Required]
        [Range(1, Double.PositiveInfinity)]
        public int UploadInterval { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}