﻿using SensorsManager.DomainClasses;
using System.Collections.Generic;

namespace SensorsManager.Web.Api.Models
{
    public class UserModelGet
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
    }
}