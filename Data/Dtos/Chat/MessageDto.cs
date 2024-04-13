using WebApi.Data.Dtos.Auth;

namespace WebApi.Data.Dtos.Chat;

public record MessageDto(int TripId, string Content, DateTime SentAt, UserDto User);
