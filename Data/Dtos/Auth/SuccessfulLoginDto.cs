namespace WebApi.Data.Dtos.Auth;

public record SuccessfulLoginDto(string AccessToken, string RefreshToken);