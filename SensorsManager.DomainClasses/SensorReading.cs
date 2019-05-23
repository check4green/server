using System;

namespace SensorsManager.DomainClasses
{
    public class SensorReading
    {
        public int Id { get; set; }
        public int Sensor_Id { get; set; }
        public string GatewayAddress { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset ReadingDate { get; set; }
        public DateTimeOffset InsertDate { get; set; }

        public Sensor Sensor { get; set; }
    }
}
