using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class GatewayModelPost
    {
        [Required, StringLength(10)]
        public string Address { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}