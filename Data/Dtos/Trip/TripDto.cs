namespace WebApi.Data.Dtos.Trip;

public record TripDto(int Id, string Departure, string Destination, string Description, DateTime Date, int Seats, int SeatsTaken, decimal Price);