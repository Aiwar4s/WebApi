using System.ComponentModel.DataAnnotations;
using WebApi.Auth.Entity;

namespace WebApi.Data.Entities;

public class Rating
{
    public int Id { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
    public Trip Trip { get; set; }

    [Required]
    public required string UserId { get; set; }

    public User User { get; set; }
}