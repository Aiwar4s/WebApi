using Microsoft.AspNetCore.Identity;
using WebApi.Data.Entities;

namespace WebApi.Auth.Entity;

public class User : IdentityUser
{
    public string? RefreshToken { get; set; }

    public ICollection<UserTrip> UserTrips { get; set; }
}
