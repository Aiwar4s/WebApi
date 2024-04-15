using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Auth.Entity;
using WebApi.Data;
using WebApi.Data.Dtos.Chat;
using WebApi.Data.Entities;

namespace WebApi;

[Authorize(Roles = UserRoles.BasicUser)]
public class ChatHub : Hub
{
    private readonly RideShareDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public ChatHub(RideShareDbContext dbContext, UserManager<User> userManager, IMapper mapper)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task JoinTripChat(int tripId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"trip-{tripId}");
    }

    public async Task LeaveTripChat(int tripId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"trip-{tripId}");
    }

    public async Task SendMessage(int tripId, string content)
    {
        string? userId = Context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        User? user = await _userManager.FindByIdAsync(userId);
        Trip? trip = await _dbContext.Trips.Include(t => t.UserTrips).Include(t => t.User).FirstOrDefaultAsync(t => t.Id == tripId);

        if (user == null || trip == null)
        {
            throw new HubException("User or trip not found");
        }

        if (trip.User.Id != user.Id && trip.UserTrips.All(x => x.UserId != user.Id))
        {
            throw new HubException("User is not part of the trip");
        }

        Message message = new Message
        {
            Content = content,
            User = user,
            Trip = trip,
            SentAt = DateTime.Now
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();

        MessageDto messageDto = _mapper.Map<MessageDto>(message);
        await Clients.Group($"trip-{tripId}").SendAsync("ReceiveMessage", messageDto);
    }
}
