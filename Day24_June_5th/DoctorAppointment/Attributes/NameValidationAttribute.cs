using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace DoctorAppointment.Attributes
{
    public class NameValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var name = value as string;

            if (string.IsNullOrWhiteSpace(name))
            {
                return new ValidationResult("Name is required.");
            }

            if (name.Length < 2 || name.Length > 50)
            {
                return new ValidationResult("Name must be between 2 and 50 characters long.");
            }


            if (!Regex.IsMatch(name, @"^[a-zA-Z\s]+$"))
            {
                return new ValidationResult("Name can contain only letters and spaces.");
            }


            return ValidationResult.Success;
        }
    }
}
