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
        
        [Required, MaxLength(50)]
        public string Name { get; set; }

        public DateTime ProductionDate { get; set; }

        [Required]
        public int UploadInterval { get; set; }

        [Required,MaxLength(4)]
        public string GatewayAddress { get; set; }

        [Required,MaxLength(4)]
        public string ClientAddress { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }



        public bool Active { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

      
        public  User User { get; set; }
        public  SensorType SensorType { get; set; }
    }
}