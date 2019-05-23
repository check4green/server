using System.Collections.Generic;

namespace SensorsManager.DomainClasses
{
    public class SensorType
    {
        public int Id { get; set; }
        public int Measure_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        
        public Measurement Measurement { get; set; }
        public List<Sensor> Sensors { get; set; }
    }
}
