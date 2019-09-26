using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class NetworkModel
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
    }
}