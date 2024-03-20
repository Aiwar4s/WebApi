using Microsoft.EntityFrameworkCore;
using WebApi.Data.Entities;

namespace WebApi.Data;

public class RideShareDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Trip?> Trips { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    public RideShareDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL(_configuration.GetConnectionString("MySQL"));
    }
}