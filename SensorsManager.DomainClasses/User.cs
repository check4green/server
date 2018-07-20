

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.DomainClasses
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required,MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        [Required, MaxLength(50)]
        public string Password { get; set; }
        [Required, MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(100)]
        public string CompanyName { get; set; }

        [Required,MaxLength(50)]
        public string Country { get; set; }

        [Required, MaxLength(13)]
        public string PhoneNumber { get; set; }

        List<Sensor> Sensors { get; set; }
    } 
}
