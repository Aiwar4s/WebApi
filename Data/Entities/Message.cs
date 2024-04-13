using WebApi.Auth.Entity;

namespace WebApi.Data.Entities;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; }
    public User User { get; set; }
    public string UserId { get; set; }
    public Trip Trip { get; set; }
    public int TripId { get; set; }
    public DateTime SentAt { get; set; }
}
