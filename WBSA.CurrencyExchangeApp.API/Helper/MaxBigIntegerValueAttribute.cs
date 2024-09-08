using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace WBSA.CurrencyExchangeApp.API.Helper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MaxBigIntegerValueAttribute : Attribute
    {
        public BigInteger MaxValue { get; }

        public MaxBigIntegerValueAttribute(string maxValue)
        {
            // Convert the string to a BigInteger
            MaxValue = BigInteger.Parse(maxValue);
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MinValueAttribute : ValidationAttribute
    {
        private readonly decimal _minValue;

        public MinValueAttribute(double minValue)
        {
            _minValue = (decimal)minValue;
            ErrorMessage = $"The value must be at least {_minValue}.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && (decimal)value < _minValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
