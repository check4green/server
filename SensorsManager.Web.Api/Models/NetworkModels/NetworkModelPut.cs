using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class NetworkModelPut
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
    }
}