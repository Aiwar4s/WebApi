namespace WebApi.Data.Dtos.Payment;

public record ChargeDto(string ChargeId, string Currency, long Amount, string CustomerId, string ReceiptEmail, string Description);
