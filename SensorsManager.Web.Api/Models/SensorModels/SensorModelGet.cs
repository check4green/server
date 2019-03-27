using System;

namespace SensorsManager.Web.Api.Models
{
    public class SensorModelGet
    {
        public string Name { get; set; }
        public int SensorTypeId { get; set; }
        public DateTime ProductionDate { get; set; }
        public int UploadInterval { get; set; }
        public string GatewayAddress { get; set; }
        public string ClientAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Active { get; set; }
    }
}