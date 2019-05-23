using System;

namespace SensorsManager.DomainClasses
{
    public class GatewayConnection
    {
        public int Gateway_Id { get; set; }
        public int Sensor_Id { get; set; }

        public Gateway Gateway { get; set; }
    }
}
