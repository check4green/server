﻿using SensorsManager.Web.Api.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorModel2
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
        [Range(1, Double.PositiveInfinity)]
        public int BatchSize { get; set; }
    }
}