namespace WebApi.Data.Dtos.Payment;

public record CreateChargeDto(string Currency, long Amount, string CustomerId, string ReceiptEmail, string Description);