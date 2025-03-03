using System;
using System.Globalization;
using System.Windows.Controls;

namespace ECDH.Validators
{
    public class ParameterValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (int.TryParse(value.ToString(), out _))
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "");
        }
    }
}
