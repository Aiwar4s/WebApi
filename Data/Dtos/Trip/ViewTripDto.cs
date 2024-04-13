using WebApi.Data.Dtos.Auth;

namespace WebApi.Data.Dtos.Trip;

public record ViewTripDto(int Id, string Departure, string Destination, string Description, DateTime Date, int Seats, int SeatsTaken, decimal Price, UserDto User, List<UserTripDto> UserTrips);