using SensorsManager.Web.Api.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class NetworkModelPost
    {
        public int User_Id { get; set; }
        public string Address { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
    }
}