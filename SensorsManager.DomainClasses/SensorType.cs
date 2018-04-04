using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorsManager.DomainClasses
{
    public class SensorType
    {
        [Required]
        [Key]
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
        [ForeignKey("Measurement")]
        public int MeasureId { get; set; }
        [Required]
        public decimal Multiplier { get; set; }

        public  Measurement Measurement { get; set; }
    }
}
