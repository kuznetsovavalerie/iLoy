using System;
using System.ComponentModel.DataAnnotations;

namespace ILoyInterview.Contracts.ApiModels.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DateAfterAttribute : ValidationAttribute
    {
        private readonly string earlierDatePropertyName;

        public DateAfterAttribute(string earlierDatePropertyName, string errorMessage = "DateAfter must be greater than date specified")
            : base(errorMessage)
        {
            this.earlierDatePropertyName = earlierDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = ValidationResult.Success;

            var earlierDatePropertyInfo = validationContext.ObjectType.GetProperty(this.earlierDatePropertyName);

            var endDateValue = (DateTime?)value;
            var startDateValue = (DateTime?)earlierDatePropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (endDateValue.Value.CompareTo(startDateValue.Value) < 1)
            {
                result = new ValidationResult(ErrorMessageString);
            }

            return result;
        }
    }
}
