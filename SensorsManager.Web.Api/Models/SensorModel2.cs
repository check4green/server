using SensorsManager.Web.Api.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorModel2
    {
        //CustomValidation(typeof(SensorValidation), "NameValidation")
        [Required, MaxLength(50)]
        [RegularExpression("^[a-zA-Z0-9_-]*$", 
            ErrorMessage = "Only alphabets, numbers and the simbols: - or _ are allowed."),
            CustomValidation(typeof(SensorValidation), "NameValidation")]
        public string Name { get; set; }
        [Required]
        public int UploadInterval { get; set; }
        [Required]
        public int BatchSize { get; set; }
    }
}