using AutoMapper;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using WebApi.Data;
using WebApi.Data.Dtos.Trip;
using WebApi.Data.Entities;

namespace WebApi;

public static class TripEndpoints
{
    public static void AddTripApi(this WebApplication app)
    {
        RouteGroupBuilder tripsGroup = app.MapGroup("/api/trips").WithValidationFilter().WithTags("Trips");
        tripsGroup.MapGet("", async (RideShareDbContext dbContext, IMapper mapper) =>
        {
            return Results.Ok((await dbContext.Trips.ToListAsync()).Select(x => mapper.Map<TripDto>(x)));
        }).WithName("GetAllTrips");

        tripsGroup.MapGet("{id}", async (RideShareDbContext dbContext, IMapper mapper, int id) =>
        {
            Trip? trip = await dbContext.Trips.FindAsync(id);
            if (trip is null) return Results.NotFound();

            return Results.Ok(mapper.Map<TripDto>(trip));
        }).WithName("GetTrip");

        tripsGroup.MapPost("", async (RideShareDbContext dbContext, IMapper mapper, [Validate] CreateTripDto tripDto) =>
        {
            Trip? trip = mapper.Map<Trip>(tripDto);
            dbContext.Trips.Add(trip);
            await dbContext.SaveChangesAsync();
            return Results.Created($"/api/trips/{trip.Id}", mapper.Map<TripDto>(trip));
        }).WithName("CreateTrip");

        tripsGroup.MapPut("{id}", async (RideShareDbContext dbContext, IMapper mapper, int id, [Validate] EditTripDto tripDto) =>
        {
            Trip? trip = await dbContext.Trips.FindAsync(id);
            if (trip is null) return Results.NotFound();

            mapper.Map(tripDto, trip);
            await dbContext.SaveChangesAsync();
            return Results.Ok(mapper.Map<TripDto>(trip));
        }).WithName("EditTrip");

        tripsGroup.MapDelete("{id}", async (RideShareDbContext dbContext, int id) =>
        {
            Trip? trip = await dbContext.Trips.FindAsync(id);
            if (trip is null) return Results.NotFound();

            dbContext.Trips.Remove(trip);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).WithName("DeleteTrip");
    }
}