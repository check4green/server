using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class UserModel_Validation
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
    }
}