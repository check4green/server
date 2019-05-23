using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class GatewayModelPut
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}