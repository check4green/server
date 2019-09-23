using System;
using System.Collections.Generic;


namespace SensorsManager.DomainClasses
{
    public class Gateway
    {
        public int Id { get; set; }
        public int Network_Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime? LastSignalDate { get; set; }
        public DateTimeOffset? LastSensorDate { get; set; }
        public int UploadInterval { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Active { get; set; }

        public Network Network { get; set; }
        public List<GatewayConnection> Connections { get; set; }
    }
}