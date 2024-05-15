using WebApi.Data.Dtos.Auth;

namespace WebApi.Data.Dtos.Rating;

public record RatingDto(int Id, int Stars, string? Comment, UserDto User, UserDto RatedUser, DateTime CreatedAt);