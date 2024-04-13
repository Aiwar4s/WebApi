using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Auth.Entity;
using WebApi.Data.Entities;

namespace WebApi.Data;

public class RideShareDbContext : IdentityDbContext<User>
{
    private readonly IConfiguration _configuration;
    public DbSet<Trip?> Trips { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<UserTrip> UserTrips { get; set; }
    public DbSet<Message> Messages { get; set; }

    public RideShareDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL(_configuration.GetConnectionString("MySQL"));
    }
}