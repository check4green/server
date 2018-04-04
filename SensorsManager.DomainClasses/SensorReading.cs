using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorsManager.DomainClasses
{
    public class SensorReading
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Sensor")]
        public int SensorId { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        public DateTimeOffset ReadingDate { get; set; }
       
        public DateTimeOffset InsertDate { get; set; }
 
        public  Sensor Sensor { get; set; }
    }
}
