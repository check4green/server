using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SensorsManager.Web.Api.Validations
{
    public class SensorValidation : ValidationAttribute
    {
        public static ValidationResult NameValidation(string name, ValidationContext validationContext)
        {
            if (name != null)
            {
                if (name.Any(c => Char.IsLetter(c)))
                {
                    if (name.Any(c => Char.IsWhiteSpace(c)))
                    {
                        return new ValidationResult("The name cannot contain whitespace.");
                    }
                    else if (!CheckSimbols(name))
                    {
                        return new ValidationResult("The name cannot start or end with a simbol." +
                            "There also cannot be two consecutive simbols.");
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return new ValidationResult("The name must contain at least a letter.");
                }
            }
            else
            {
                return new ValidationResult("The name field is required.");
            }
        }

        public static bool CheckSimbols(string name)
        {
            for (int i = 0; i < name.Length; i++)
            {
                if (Char.IsPunctuation(name[0])
                    || Char.IsPunctuation(name[name.Length - 1]))
                {
                    return false;
                }
                else if (i + 1 < name.Length)
                {
                    if ((Char.IsPunctuation(name[i])
                        && Char.IsPunctuation(name[i + 1])))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}