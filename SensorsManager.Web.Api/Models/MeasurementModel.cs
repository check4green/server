using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Models
{
    public class MeasurementModel
    {
        public int Id { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Description { get; set; }
    }
}