using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Auth.Entity;
using WebApi.Data;
using WebApi.Data.Dtos.Chat;
using WebApi.Data.Entities;

namespace WebApi.Endpoints;

public static class ChatEndpoints
{
    public static void AddChatApi(this WebApplication app)
    {
        RouteGroupBuilder chatGroup = app.MapGroup("/api/trips/{tripId}/messages").WithTags("Chat");

        chatGroup.MapGet("", [Authorize(Roles = UserRoles.BasicUser)] async (int tripId, RideShareDbContext dbContext, UserManager<User> userManager, HttpContext httpContext, IMapper mapper) =>
        {
            User? user = await userManager.FindByIdAsync(httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? throw new InvalidOperationException());
            Trip? trip = await dbContext.Trips.FindAsync(tripId);
            if (user == null || trip == null)
            {
                return Results.NotFound();
            }

            if (trip.UserTrips.All(x => x.UserId != user.Id))
            {
                return Results.Forbid();
            }

            List<Message> messages = await dbContext.Messages.Where(x => x.Trip.Id == tripId).OrderBy(x => x.SentAt).ToListAsync();
            return Results.Ok(messages.Select(x => mapper.Map<MessageDto>(x)));
        }).WithName("GetTripMessages");
    }
}
