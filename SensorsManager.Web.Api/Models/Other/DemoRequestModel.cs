using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class DemoRequestModel
    {
        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Company { get; set; }
        public string JobTitle { get; set; }
        public string CompanySize { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}