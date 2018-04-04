

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SensorsManager.DomainClasses
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        List<Sensor> Sensors { get; set; }
    } 
}
