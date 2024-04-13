using FluentValidation;
using WebApi.Data.Dtos.Auth;

namespace WebApi.Validators.Auth;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(dto => dto.Email).NotNull().NotEmpty().EmailAddress();
        RuleFor(dto => dto.Password).NotNull().NotEmpty().MinimumLength(7).MaximumLength(50);
    }
}
