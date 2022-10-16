using FluentValidation;
using Microsoft.Extensions.Logging;
using Cloud.$ext_safeprojectname$.API.Application.Commands;

namespace Cloud.$ext_safeprojectname$.API.Application.Validations
{
    public class Create$ext_safeprojectname$CommandValidator : AbstractValidator<Create$ext_safeprojectname$Command>
    {
        public Create$ext_safeprojectname$CommandValidator(ILogger<Create$ext_safeprojectname$CommandValidator> logger)
        {
            RuleFor(x => x.Quantity).QuantityRange();
            RuleFor(x => x.Amount).AmountRange();           
        }
    }

    public class Update$ext_safeprojectname$CommandValidator : AbstractValidator<Update$ext_safeprojectname$Command>
    {
        public Update$ext_safeprojectname$CommandValidator(ILogger<Update$ext_safeprojectname$CommandValidator> logger)
        {
            RuleFor(x => x.Quantity).QuantityRange();
            RuleFor(x => x.Amount).AmountRange();
        }
    }
}
