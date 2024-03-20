namespace WebApi.Data.Dtos.Trip;

public record CreateTripDto(string Departure, string Destination, string Description, DateTime Date, int Seats, decimal Price);