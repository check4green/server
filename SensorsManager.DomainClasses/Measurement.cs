using System.Collections.Generic;

namespace SensorsManager.DomainClasses
{
    public class Measurement
    {
        public int Id { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Description { get; set; }

        public List<SensorType> SensorType { get; set; }
    }
}
