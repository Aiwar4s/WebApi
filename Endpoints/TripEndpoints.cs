using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using O9d.AspNet.FluentValidation;
using WebApi.Auth.Entity;
using WebApi.Data;
using WebApi.Data.Dtos.Trip;
using WebApi.Data.Entities;

namespace WebApi.Endpoints;

public static class TripEndpoints
{
    public static void AddTripApi(this WebApplication app)
    {
        RouteGroupBuilder tripsGroup = app.MapGroup("/api/trips").WithValidationFilter().WithTags("Trips");

        tripsGroup.MapGet("", async (RideShareDbContext dbContext, IMapper mapper, HttpContext httpContext, string? departure, string? destination) =>
        {
            IQueryable<Trip> tripQuery = dbContext.Trips;
            tripQuery = httpContext.User.IsInRole(UserRoles.Admin)
                ? tripQuery.OrderByDescending(t => t.Date)
                : tripQuery.Where(t => t.Date > DateTime.Now && t.SeatsTaken < t.Seats).OrderBy(t => t.Date);

            if (!string.IsNullOrEmpty(departure))
            {
                tripQuery = tripQuery.Where(t => t.Departure == departure);
            }

            if (!string.IsNullOrEmpty(destination))
            {
                tripQuery = tripQuery.Where(t => t.Destination == destination);
            }

            List<Trip> trips = await tripQuery.Include(t => t.User).ThenInclude(u => u.Ratings).ToListAsync();
            return Results.Ok(trips.Select(mapper.Map<TripDto>));
        }).WithName("GetAllTrips");

        tripsGroup.MapGet("{id}", async (RideShareDbContext dbContext, IMapper mapper, int id) =>
        {
            Trip? trip = await dbContext.Trips
                                        .Include(t => t.User)
                                        .ThenInclude(u => u.Ratings)
                                        .Include(t => t.UserTrips)
                                        .ThenInclude(ut => ut.User)
                                        .ThenInclude(u => u.Ratings)
                                        .FirstOrDefaultAsync(t => t.Id == id);
            if (trip is null) return Results.NotFound();

            return Results.Ok(mapper.Map<ViewTripDto>(trip));
        }).WithName("GetTrip");

        tripsGroup.MapGet("joined", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, IMapper mapper, HttpContext httpContext) =>
        {
            User? user = await dbContext.Users.FindAsync(httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            if (user is null) return Results.NotFound();

            List<Trip> trips = await dbContext.Trips
                                              .Include(t => t.User)
                                              .Include(t => t.UserTrips)
                                              .ThenInclude(ut => ut.User)
                                              .Where(t => t.UserTrips.Any(ut => ut.UserId == user.Id))
                                              .OrderByDescending(t => t.Date)
                                              .ToListAsync();

            return Results.Ok(trips.Select(mapper.Map<TripDto>));
        }).WithName("GetJoinedTrips");

        tripsGroup.MapGet("created", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, IMapper mapper, HttpContext httpContext) =>
        {
            User? user = await dbContext.Users.FindAsync(httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            if (user is null) return Results.NotFound();

            List<Trip> trips = await dbContext.Trips
                                              .Include(t => t.User)
                                              .Where(t => t.UserId == user.Id)
                                              .OrderByDescending(t => t.Date)
                                              .ToListAsync();

            return Results.Ok(trips.Select(mapper.Map<TripDto>));
        }).WithName("GetCreatedTrips");

        tripsGroup.MapPost("", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, IMapper mapper, [Validate] CreateTripDto tripDto, HttpContext httpContext) =>
        {
            Trip trip = mapper.Map<Trip>(tripDto);
            trip.UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            dbContext.Trips.Add(trip);
            await dbContext.SaveChangesAsync();
            return Results.Created($"/api/trips/{trip.Id}", mapper.Map<TripDto>(trip));
        }).WithName("CreateTrip");

        tripsGroup.MapPost("{tripId}/join", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, IMapper mapper, int tripId, [Validate] JoinTripDto joinTripDto, HttpContext httpContext) =>
        {
            User? user = await dbContext.Users.FindAsync(httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            Trip? trip = await dbContext.Trips
                                        .Include(t => t.User)
                                        .Include(t => t.UserTrips)
                                        .ThenInclude(ut => ut.User)
                                        .FirstOrDefaultAsync(t => t.Id == tripId);
            if (user == null || trip == null)
            {
                return Results.NotFound();
            }

            if (trip.SeatsTaken + joinTripDto.Seats > trip.Seats)
            {
                return Results.BadRequest("Not enough seats available");
            }

            if (trip.UserId == user.Id)
            {
                return Results.BadRequest("You can't join your own trip");
            }

            UserTrip userTrip = new UserTrip
            {
                Trip = trip,
                User = user,
                Seats = joinTripDto.Seats
            };
            trip.SeatsTaken += joinTripDto.Seats;
            dbContext.UserTrips.Add(userTrip);
            await dbContext.SaveChangesAsync();

            return Results.Ok(mapper.Map<ViewTripDto>(trip));
        }).WithName("JoinTrip");

        tripsGroup.MapPut("{id}", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, IMapper mapper, int id, EditTripDto tripDto, HttpContext httpContext) =>
        {
            Trip? trip = await dbContext.Trips
                                        .Include(t => t.User)
                                        .ThenInclude(u => u.Ratings)
                                        .Include(t => t.UserTrips)
                                        .ThenInclude(ut => ut.User)
                                        .ThenInclude(u => u.Ratings)
                                        .FirstOrDefaultAsync(t => t.Id == id);
            if (trip is null) return Results.NotFound();

            if (trip.UserId != httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) || DateTime.Parse(tripDto.Date) < DateTime.Now.AddHours(1))
            {
                return Results.Forbid();
            }

            trip.Description = tripDto.Description;
            trip.Date = DateTime.Parse(tripDto.Date);
            await dbContext.SaveChangesAsync();
            return Results.Ok(mapper.Map<ViewTripDto>(trip));
        }).WithName("EditTrip");

        tripsGroup.MapDelete("{id}", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, int id) =>
        {
            Trip? trip = await dbContext.Trips.FindAsync(id);
            if (trip is null) return Results.NotFound();

            dbContext.Trips.Remove(trip);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).WithName("DeleteTrip");

        tripsGroup.MapDelete("{tripId}/leave", [Authorize(Roles = UserRoles.BasicUser)] async (RideShareDbContext dbContext, IMapper mapper, int tripId, HttpContext httpContext) =>
        {
            User? user = await dbContext.Users.FindAsync(httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            Trip? trip = await dbContext.Trips
                                        .Include(t => t.User).Include(t => t.UserTrips).ThenInclude(ut => ut.User).FirstOrDefaultAsync(t => t.Id == tripId);
            if (user == null || trip == null)
            {
                return Results.NotFound();
            }

            UserTrip? userTrip = trip.UserTrips.FirstOrDefault(ut => ut.UserId == user.Id);
            // if (userTrip == null || trip.Date < DateTime.Now)
            if (userTrip == null)
            {
                return Results.Forbid();
            }

            trip.SeatsTaken -= userTrip.Seats;
            dbContext.UserTrips.Remove(userTrip);
            await dbContext.SaveChangesAsync();
            return Results.Ok(mapper.Map<ViewTripDto>(trip));
        }).WithName("LeaveTrip");
    }
}