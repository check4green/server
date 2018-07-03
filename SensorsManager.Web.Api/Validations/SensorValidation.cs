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
            if (name != null)
            {
                if (name.Any(c => Char.IsWhiteSpace(c)))
                {
                    return new ValidationResult("Name cannot contain whitespace");
                }
                else if (name.Any(c => Char.IsLetter(c)))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Name must contain at least a letter.");
                }
            }
            else
            {
                return new ValidationResult("Name field is required.");
            }
        }

      
    }
}