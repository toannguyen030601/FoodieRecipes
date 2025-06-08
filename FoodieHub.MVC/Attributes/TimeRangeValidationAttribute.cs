using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Attributes
{
    public class TimeRangeValidationAttribute : ValidationAttribute
    {
        private readonly TimeOnly _minTime;
        private readonly TimeOnly _maxTime;

        public TimeRangeValidationAttribute(string minTime, string maxTime)
        {
            _minTime = TimeOnly.ParseExact(minTime, "HH:mm");
            _maxTime = TimeOnly.ParseExact(maxTime, "HH:mm");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is TimeOnly time)
            {
                if (time < _minTime || time > _maxTime)
                {
                    return new ValidationResult(ErrorMessage ?? $"Time must be between {_minTime:HH:mm} and {_maxTime:HH:mm}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
