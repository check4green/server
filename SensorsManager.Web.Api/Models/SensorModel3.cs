using SensorsManager.Web.Api.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorModel3
    {
        
        [Required, MaxLength(50),RegularExpression("^[a-zA-Z0-9_-]*$",
            ErrorMessage = "Only alphabets, numbers and the simbols: - or _ are allowed."),
            CustomValidation(typeof(SensorValidation), "NameValidation")]
        public string Name { get; set; }
        [Required]
        public int SensorTypeId { get; set; }
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public int UploadInterval { get; set; }
        [Required]
        public int BatchSize { get; set; }
        [Required, MaxLength(4)]
        [RegularExpression("0x+[a-fA-F0-9]+[a-fA-F0-9]", 
            ErrorMessage ="Address must match the hexadecimal format: 0x__; [a-f or 0-9].")]
        public string GatewayAddress { get; set; }
        [Required, MaxLength(4)]
        [RegularExpression("0x+[a-fA-F0-9]+[a-fA-F0-9]",
            ErrorMessage = "Address must match the hexadecimal format: 0x__; [a-f or 0-9].")]
        public string ClientAddress { get; set; }

       

      
    }
}