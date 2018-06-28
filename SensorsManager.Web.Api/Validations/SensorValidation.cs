using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Validations
{
    public class SensorValidation : ValidationAttribute
    {
        public static ValidationResult NameValidation(string name, ValidationContext validationContext)
        {
           if(name.Any(c => Char.IsWhiteSpace(c)))
            {
                return new ValidationResult("Name cannot contain whitespace");
            }
           else if(name.Any(c => Char.IsLetter(c)))
            {
                return ValidationResult.Success;
            }
           else
            {
                return new ValidationResult("Name must contain at least a letter.");
            }
        }

        //public static ValidationResult AddressValidation(string address)
        //{
        //    if (address.Length == 4)
        //    {
        //        if (address.Substring(0, 2) == "0x")
        //        {
        //            if (int.TryParse(address.Substring(2, 2),
        //                     System.Globalization.NumberStyles.HexNumber,
        //                     System.Globalization.CultureInfo.InvariantCulture, out int res))
        //            {
        //                return ValidationResult.Success;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}
    }
}