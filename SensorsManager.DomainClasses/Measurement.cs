

using System.ComponentModel.DataAnnotations;

namespace SensorsManager.DomainClasses
{
    public class Measurement
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string UnitOfMeasure { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
    }
}
