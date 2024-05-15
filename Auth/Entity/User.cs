using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using WebApi.Data.Entities;

namespace WebApi.Auth.Entity;

public class User : IdentityUser
{
    public User()
    {
        Ratings = new List<Rating>();
    }

    public string? RefreshToken { get; set; }

    [NotMapped]
    public ICollection<Rating> Ratings { get; set; }

    [NotMapped]
    public List<Rating> ReceivedRatings => Ratings.Where(r => r.RatedUserId == Id).ToList();

    public decimal AverageRating
    {
        get
        {
            if (!ReceivedRatings.Any())
            {
                return 0;
            }

            return (decimal)ReceivedRatings.Average(r => r.Stars);
        }
    }
}
