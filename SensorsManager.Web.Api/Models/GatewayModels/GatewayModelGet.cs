using System;

namespace SensorsManager.Web.Api.Models
{
    public class GatewayModelGet
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTimeOffset? LastSensorDate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Active { get; set; }
    }
}