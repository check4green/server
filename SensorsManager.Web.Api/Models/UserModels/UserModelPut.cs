using System.ComponentModel.DataAnnotations;

namespace SensorsManager.Web.Api.Models
{
    public class UserModelPut
    {
        [Required, MaxLength(50)]
        [RegularExpression("^[a-zA-Z-.]*$",
            ErrorMessage = "Only alphabets, numbers and the simbols: - and . are allowed.")]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        [RegularExpression("^[a-zA-Z-.]*$",
            ErrorMessage = "Only alphabets, numbers and the simbols: - and . are allowed.")]
        public string LastName { get; set; }

        [Required, MaxLength(50)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [RegularExpression("^[a-zA-Z0-9-.]*$",
            ErrorMessage = "Only alphabets, numbers and the simbols: - and . are allowed.")]
        [MaxLength(100)]
        public string CompanyName { get; set; }

        [RegularExpression("^[a-zA-Z0-9]*$",
            ErrorMessage = "Invalid country name.")]
        [Required, MaxLength(50)]
        public string Country { get; set; }
        [MaxLength(13, ErrorMessage = "The field PhoneNumber has a maximum of '13'")]
        [Phone(ErrorMessage = "Wrong phone format.")]
        public string PhoneNumber { get; set; }
    }
}