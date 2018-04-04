using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorsManager.DomainClasses
{
    public class Sensor
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("SensorType")]
        public int SensorTypeId { get; set; }

        public DateTime ProductionDate { get; set; }
        [Required]
        public int UploadInteval { get; set; }
        [Required]
        public int BatchSize { get; set; }
     
        [ForeignKey("User")]
        public int UserId { get; set; }


        public  User User { get; set; }
        public  SensorType SensorType { get; set; }
    }
}