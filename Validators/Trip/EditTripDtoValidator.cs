using FluentValidation;
using WebApi.Data.Dtos.Trip;

namespace WebApi.Validators.Trip;

public class EditTripDtoValidator : AbstractValidator<EditTripDto>
{
    public EditTripDtoValidator()
    {
        RuleFor(dto => dto.Date).NotNull().GreaterThan(DateTime.Now.AddHours(2));
        RuleFor(dto => dto.Seats).NotNull().GreaterThan(0).LessThanOrEqualTo(20);
        RuleFor(dto => dto.Price).NotNull().GreaterThanOrEqualTo(0);
    }
}