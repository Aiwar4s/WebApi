using WebApi.Data.Dtos.Auth;

namespace WebApi.Data.Dtos.Trip;

public record UserTripDto(int Id, int Seats, int TripId, UserDto User);
