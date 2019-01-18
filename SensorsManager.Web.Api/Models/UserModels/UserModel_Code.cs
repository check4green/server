using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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