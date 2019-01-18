using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class ContactInfoModel
    {
        [Required, MaxLength(50)]
        public string FullName { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required, Phone]
        public string Phone { get; set; }
        [Required, MaxLength(500)]
        public string Message { get; set; }
    }
}