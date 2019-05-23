using System.Collections.Generic;

namespace SensorsManager.DomainClasses
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }

        public List<Network> Networks { get; set; }
    } 
}
