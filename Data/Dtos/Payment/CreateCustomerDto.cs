namespace WebApi.Data.Dtos.Payment;

public record CreateCustomerDto(string Email, string Name, CreateCardDto Card);
