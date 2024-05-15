using WebApi.Data.Dtos.Rating;

namespace WebApi.Data.Dtos.Profile;

public record ProfileDto(string Id, string Username, decimal AverageRating, List<RatingDto> Ratings, int TripsCount, int UserTripsCount);