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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rating>()
                    .HasOne(r => r.RatingUser)
                    .WithMany()
                    .HasForeignKey(r => r.RatingUserId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Rating>()
                    .HasOne(r => r.RatedUser)
                    .WithMany(u => u.Ratings)
                    .HasForeignKey(r => r.RatedUserId)
                    .OnDelete(DeleteBehavior.Cascade);
    }
}