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
        [ForeignKey("Sensor"), Column(Order = 0)]
        public int SensorId { get; set; }

        [MaxLength(4)]
        [ForeignKey("Sensor"), Column(Order = 1)]
        public string SensorGatewayAdress { get; set; }

        [MaxLength(4)]
        [ForeignKey("Sensor"), Column(Order = 2)]
        public string SensorClientAdress { get; set; }

        [Required]
        public decimal Value { get; set; }
        [Required]
        public DateTimeOffset ReadingDate { get; set; }

        public DateTimeOffset InsertDate { get; set; }

        public Sensor Sensor { get; set; }
    }
}
