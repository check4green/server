using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class SensorTypeModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        [Required]
        public decimal MinValue { get; set; }
        [Required]
        public decimal MaxValue { get; set; }
        [Required]
        public int MeasureId { get; set; }
        [Required]
        public decimal Multiplier { get; set; }
    }
}