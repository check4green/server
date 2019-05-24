using System;

namespace SensorsManager.Web.Api.Models
{
    public class SensorModelGet
    {
        public int Id { get; set; }
        public int SensorTypeId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTimeOffset? LastReadingDate { get; set; }
        public int UploadInterval { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Active { get; set; }
    }
}