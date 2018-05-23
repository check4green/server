using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorsManager.DomainClasses
{
    public class Sensor
    {
        [Required]
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("SensorType")]
        public int SensorTypeId { get; set; }

        public DateTime ProductionDate { get; set; }
        [Required]
        public int UploadInterval { get; set; }
        [Required]
        public int BatchSize { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(4)]
        public string GatewayAddress { get; set; }

        [Key]
        [Column(Order = 2)]
        [MaxLength(4)]
        public string ClientAddress { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

      
        public  User User { get; set; }
        public  SensorType SensorType { get; set; }
    }
}