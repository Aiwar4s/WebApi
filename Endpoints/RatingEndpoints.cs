using AutoMapper;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using WebApi.Data;
using WebApi.Data.Dtos.Rating;
using WebApi.Data.Entities;

namespace WebApi.Endpoints;

public static class RatingEndpoints
{
    public static void AddRatingApi(this WebApplication app)
    {
        RouteGroupBuilder routesGroup = app.MapGroup("/api/trips/{tripId}/ratings").WithValidationFilter().WithTags("Ratings");
        routesGroup.MapGet("", async (RideShareDbContext dbContext, IMapper mapper, int tripId) =>
        {
            Trip? trip = await dbContext.Trips.FindAsync(tripId);
            if (trip is null) return Results.NotFound();

            return Results.Ok((await dbContext.Ratings.Where(x => x.Trip.Id == tripId).ToListAsync()).Select(x => mapper.Map<RatingDto>(x)));
        }).WithName("GetAllRatings");

        routesGroup.MapGet("{id}", async (RideShareDbContext dbContext, IMapper mapper, int tripId, int id) =>
        {
            Trip? trip = await dbContext.Trips.FindAsync(tripId);
            if (trip is null) return Results.NotFound();

            Rating? rating = await dbContext.Ratings.FirstOrDefaultAsync(x => x.Trip.Id == tripId && x.Id == id);
            if (rating is null) return Results.NotFound();

            return Results.Ok(mapper.Map<RatingDto>(rating));
        }).WithName("GetRating");

        routesGroup.MapPost("", async (RideShareDbContext dbContext, IMapper mapper, int tripId, [Validate] CreateRatingDto ratingDto, HttpContext httpContext) =>
        {
            Trip? trip = await dbContext.Trips.FindAsync(tripId);
            if (trip is null) return Results.NotFound();

            Rating rating = mapper.Map<Rating>(ratingDto);
            rating.UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            rating.Trip = trip;
            dbContext.Ratings.Add(rating);
            await dbContext.SaveChangesAsync();
            return Results.Created($"/api/trips/{tripId}/ratings/{rating.Id}", mapper.Map<RatingDto>(rating));
        }).WithName("CreateRating");

        routesGroup.MapPut("{id}", async (RideShareDbContext dbContext, IMapper mapper, int tripId, int id, [Validate] CreateRatingDto ratingDto) =>
        {
            Trip? trip = await dbContext.Trips.FindAsync(tripId);
            if (trip is null) return Results.NotFound();

            Rating? rating = await dbContext.Ratings.FirstOrDefaultAsync(x => x.Trip.Id == tripId && x.Id == id);
            if (rating is null) return Results.NotFound();

            mapper.Map(ratingDto, rating);
            await dbContext.SaveChangesAsync();
            return Results.Ok(mapper.Map<RatingDto>(rating));
        }).WithName("EditRating");

        routesGroup.MapDelete("{id}", async (RideShareDbContext dbContext, int tripId, int id) =>
        {
            Trip? trip = await dbContext.Trips.FindAsync(tripId);
            if (trip is null) return Results.NotFound();

            Rating? rating = await dbContext.Ratings.FirstOrDefaultAsync(x => x.Trip.Id == tripId && x.Id == id);
            if (rating is null) return Results.NotFound();

            dbContext.Ratings.Remove(rating);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).WithName("DeleteRating");
    }
}
