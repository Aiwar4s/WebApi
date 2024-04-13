using FluentValidation;
using WebApi.Data.Dtos.Auth;

namespace WebApi.Validators.Auth;

public class LogoutDtoValidator : AbstractValidator<LogoutDto>
{
    public LogoutDtoValidator()
    {
        RuleFor(dto => dto.Username).NotNull().NotEmpty();
    }
}
