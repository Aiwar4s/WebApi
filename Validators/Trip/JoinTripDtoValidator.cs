using FluentValidation;
using WebApi.Data.Dtos.Trip;

namespace WebApi.Validators.Trip;

public class JoinTripDtoValidator : AbstractValidator<JoinTripDto>
{
    public JoinTripDtoValidator()
    {
        RuleFor(dto => dto.Seats).NotNull().GreaterThan(0);
    }
}
