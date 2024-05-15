using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.Auth.Entity;
using WebApi.Data;
using WebApi.Data.Dtos.Profile;
using WebApi.Data.Dtos.Rating;

namespace WebApi.Endpoints;

public static class ProfileEndpoints
{
    public static void AddProfileApi(this WebApplication app)
    {
        RouteGroupBuilder profileGroup = app.MapGroup("/api/profile/{userId}").WithTags("Profile");

        profileGroup.MapGet("", async (RideShareDbContext dbContext, IMapper mapper, string userId) =>
        {
            User? user = await dbContext.Users
                                        .Include(u => u.Ratings)
                                        .ThenInclude(r => r.RatingUser)
                                        .FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null) return Results.NotFound();

            int tripsCount = await dbContext.Trips.CountAsync(t => t.UserId == userId);
            int userTripsCount = await dbContext.UserTrips.CountAsync(ut => ut.UserId == userId);
            return Results.Ok(new ProfileDto(user.Id, user.UserName, user.AverageRating, user.ReceivedRatings.Select(r => mapper.Map<RatingDto>(r)).ToList(), tripsCount, userTripsCount));
        }).WithName("GetProfile");
    }
}