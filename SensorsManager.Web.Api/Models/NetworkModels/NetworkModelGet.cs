using System;

namespace SensorsManager.Web.Api.Models
{
    public class NetworkModelGet
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public DateTime ProductionDate { get; set; }
    }
}