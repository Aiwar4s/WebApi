using FluentValidation;
using WebApi.Data.Dtos.Auth;

namespace WebApi.Validators.Auth;

public class RefreshAccessTokenDtoValidator : AbstractValidator<RefreshAccessTokenDto>
{
    public RefreshAccessTokenDtoValidator()
    {
        RuleFor(dto => dto.RefreshToken).NotNull().NotEmpty();
    }
}
