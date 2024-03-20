namespace WebApi.Data.Dtos.Trip;

public record EditTripDto(string Description, DateTime Date, int Seats, decimal Price);