using System;
using System.Collections.Generic;

namespace SensorsManager.DomainClasses
{
    public class Network
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public int User_Id { get; set; }
        public DateTime ProductionDate { get; set; }
        public User User { get; set; }
        public List<Sensor> Sensors { get; set; }
        public List<Gateway> Gateways { get; set; }
    }
}