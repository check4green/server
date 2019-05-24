using System;
using System.Collections.Generic;

namespace SensorsManager.DomainClasses
{
    public class Sensor
    {
        public int Id { get; set; }
        public int Network_Id { get; set; }
        public int SensorType_Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTimeOffset? LastReadingDate { get; set; }
        public DateTimeOffset? LastInsertDate { get; set; }


        public int UploadInterval { get; set; }
        public bool Active { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
       

        public SensorType SensorType { get; set; }
        public Network Network { get; set; }
        public List<SensorReading> SensorReadings { get; set; }
    }
}