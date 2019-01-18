using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class MeasurementModel
    {
        public string Url { get; set; }
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string UnitOfMeasure { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
    }
}