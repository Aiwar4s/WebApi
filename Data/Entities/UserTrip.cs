using WebApi.Auth.Entity;

namespace WebApi.Data.Entities;

public class UserTrip
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }

    public int TripId { get; set; }
    public Trip Trip { get; set; }

    public int Seats { get; set; }
}