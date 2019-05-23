using System.Collections.Generic;
using System.Linq;

namespace SensorsManager.Web.Api.Models
{
    public class NetworkWithSensorsModel
    {
        public string Network { get; set; }
        public List<string> Sensors { get; set; }
    }
}