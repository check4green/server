using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorModel2
    {
        [Required]
        public int UploadInterval { get; set; }
        [Required]
        public int BatchSize { get; set; }
    }
}