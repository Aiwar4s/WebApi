using FluentValidation;
using WebApi.Data.Dtos.Auth;

namespace WebApi.Validators.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(dto => dto.Email).NotNull().NotEmpty().EmailAddress();
        RuleFor(dto => dto.Username).NotNull().NotEmpty().MinimumLength(3).MaximumLength(25);
        RuleFor(dto => dto.Password).NotNull().NotEmpty().MinimumLength(7).MaximumLength(50);
    }
}
