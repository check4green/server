using System;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorReadingModelPostAddres
    {
        [Required, MaxLength(4)]
        [RegularExpression("0x+[a-fA-F0-9]+[a-fA-F0-9]",
            ErrorMessage = "Address must match the hexadecimal format: 0x__; [a-f or 0-9].")]
        public string SensorGatewayAddress { get; set; }
        [Required, MaxLength(4)]
        [RegularExpression("0x+[a-fA-F0-9]+[a-fA-F0-9]",
            ErrorMessage = "Address must match the hexadecimal format: 0x__; [a-f or 0-9].")]
        public string SensorClientAddress { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        public DateTimeOffset ReadingDate { get; set; }


    }
}