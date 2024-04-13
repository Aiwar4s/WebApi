namespace WebApi.Data.Dtos.Payment;

public record CreateCardDto(string Name, string Number, string ExpiryYear, string ExpiryMonth, string Cvc);
