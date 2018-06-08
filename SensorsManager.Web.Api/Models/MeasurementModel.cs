using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Models
{
    public class MeasurementModel
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string UnitOfMeasure { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
    }
}