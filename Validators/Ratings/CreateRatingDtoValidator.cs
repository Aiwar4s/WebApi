using FluentValidation;
using WebApi.Data.Dtos.Rating;

namespace WebApi.Validators.Ratings;

public class CreateRatingDtoValidator : AbstractValidator<CreateRatingDto>
{
    public CreateRatingDtoValidator()
    {
        RuleFor(dto => dto.Stars).NotNull().InclusiveBetween(1, 5);
        RuleFor(dto => dto.Comment).MaximumLength(250);
    }
}
