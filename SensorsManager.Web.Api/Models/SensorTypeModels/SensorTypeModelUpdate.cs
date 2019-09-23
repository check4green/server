using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class SensorTypeModelUpdate
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        [Required]
        public decimal MinValue { get; set; }
        [Required]
        public decimal MaxValue { get; set; }
        [Required]
        public decimal Multiplier { get; set; }
    }
}