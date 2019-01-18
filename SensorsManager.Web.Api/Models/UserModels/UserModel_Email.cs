using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class UserModel_Email
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        public string Email { get; set; }
    }
}