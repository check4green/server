using SensorsManager.Web.Api.Validations;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class GatewayModelPost
    {
        public int Id { get; set; }
        public int Network_Id { get; set; }
        [Required, StringLength(10)]
        public string Address { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        public int UploadInterval { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Active { get; set; }
    }
}