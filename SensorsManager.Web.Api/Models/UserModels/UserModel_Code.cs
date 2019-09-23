using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class UserModel_Code
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
        [Required, MinLength(6), MaxLength(50)]
        public string Password { get; set; }

    }
}