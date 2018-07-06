using System;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorReadingModel2
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
        [Range(typeof(DateTime), "1/2/2018", "1/2/9999", ErrorMessage = "Value for {0} must be greater than {1}")]
        public DateTimeOffset ReadingDate { get; set; }


    }
}