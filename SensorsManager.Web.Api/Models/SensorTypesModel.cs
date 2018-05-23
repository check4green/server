using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Models
{
    public class SensorTypesModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int MeasureId { get; set; }
        public decimal Multiplier { get; set; }
    }
}