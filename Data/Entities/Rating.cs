using WebApi.Auth.Entity;

namespace WebApi.Data.Entities;

public class Rating
{
    public int Id { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
    public string RatingUserId { get; set; }
    public User RatingUser { get; set; }
    public string RatedUserId { get; set; }
    public User RatedUser { get; set; }
    public DateTime CreatedAt { get; set; }
}