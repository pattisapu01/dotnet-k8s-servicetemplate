using FluentValidation;

namespace Cloud.$ext_safeprojectname$.API.Application.Validations
{
    public static class Validators
    {
        public static IRuleBuilderOptions<T, int>
           QuantityRange<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.InclusiveBetween(1, 10).WithMessage("Quantity range must be between 1 and 10");
        }

        public static IRuleBuilderOptions<T, double>
           AmountRange<T>(this IRuleBuilder<T, double> ruleBuilder)
        {
            return ruleBuilder.InclusiveBetween(.01, 100).WithMessage("Amount range must be between .01 and 100");
        }
    }
}
