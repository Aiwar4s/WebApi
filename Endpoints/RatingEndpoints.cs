using AutoMapper;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using WebApi.Auth.Entity;
using WebApi.Data;
using WebApi.Data.Dtos.Rating;
using WebApi.Data.Entities;

namespace WebApi.Endpoints;

public static class RatingEndpoints
{
    public static void AddRatingApi(this WebApplication app)
    {
        RouteGroupBuilder ratingsGroup = app.MapGroup("/api/ratings/{userId}").WithValidationFilter().WithTags("Ratings");

        ratingsGroup.MapGet("", async (RideShareDbContext dbContext, IMapper mapper, string userId) =>
        {
            User? user = await dbContext.Users.FindAsync(userId);
            if (user is null) return Results.NotFound();

            return Results.Ok((await dbContext.Ratings.Where(x => x.RatedUserId == userId).ToListAsync()).Select(x => mapper.Map<RatingDto>(x)));
        }).WithName("GetAllRatings");

        ratingsGroup.MapGet("{id}", async (RideShareDbContext dbContext, IMapper mapper, string userId, int id) =>
        {
            User? user = await dbContext.Users.FindAsync(userId);
            if (user is null) return Results.NotFound();

            Rating? rating = await dbContext.Ratings.FirstOrDefaultAsync(x => x.RatedUserId == userId && x.Id == id);
            if (rating is null) return Results.NotFound();

            return Results.Ok(mapper.Map<RatingDto>(rating));
        }).WithName("GetRating");

        ratingsGroup.MapGet("my-rating", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, IMapper mapper, string userId, HttpContext httpContext) =>
        {
            Rating? rating = await dbContext.Ratings
                                            .Include(r => r.RatingUser)
                                            .Include(r => r.RatedUser)
                                            .FirstOrDefaultAsync(r => r.RatedUserId == userId && r.RatingUserId == httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            if (rating is null) return Results.NotFound();

            return Results.Ok(mapper.Map<RatingDto>(rating));
        }).WithName("GetMyRating");

        ratingsGroup.MapPost("", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, IMapper mapper, string userId, [Validate] CreateRatingDto ratingDto, HttpContext httpContext) =>
        {
            User? ratedUser = await dbContext.Users.FindAsync(userId);
            User? user = await dbContext.Users.FindAsync(httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            if (user == null || ratedUser == null) return Results.NotFound();
            if (user == ratedUser) return Results.BadRequest("You cannot rate yourself.");

            Rating rating = mapper.Map<Rating>(ratingDto);
            rating.RatingUser = user;
            rating.RatingUserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            rating.RatedUser = ratedUser;
            rating.RatedUserId = userId;
            rating.CreatedAt = DateTime.Now;
            dbContext.Ratings.Add(rating);
            ratedUser.Ratings.Add(rating);
            await dbContext.SaveChangesAsync();
            return Results.Created($"/api/ratings/{userId}/{rating.Id}", mapper.Map<RatingDto>(rating));
        }).WithName("CreateRating");

        ratingsGroup.MapPut("", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, IMapper mapper, string userId, [Validate] CreateRatingDto ratingDto, HttpContext httpContext) =>
        {
            Rating? rating = await dbContext.Ratings
                                            .Include(r => r.RatingUser)
                                            .Include(r => r.RatedUser)
                                            .FirstOrDefaultAsync(r => r.RatedUserId == userId && r.RatingUserId == httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            if (rating is null) return Results.NotFound();

            mapper.Map(ratingDto, rating);
            await dbContext.SaveChangesAsync();
            return Results.Ok(mapper.Map<RatingDto>(rating));
        }).WithName("EditRating");

        ratingsGroup.MapDelete("{id}", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, string userId, int id, HttpContext httpContext) =>
        {
            User? ratedUser = await dbContext.Users.FindAsync(userId);
            if (ratedUser == null) return Results.NotFound();

            Rating? rating = await dbContext.Ratings.FirstOrDefaultAsync(x => x.RatedUserId == userId && x.Id == id);
            if (rating is null) return Results.NotFound();

            if (!httpContext.User.IsInRole(UserRoles.Admin) && rating.RatingUserId != httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)) return Results.Forbid();

            dbContext.Ratings.Remove(rating);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).WithName("DeleteRating");
    }
}
