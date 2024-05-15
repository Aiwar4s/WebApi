using AutoMapper;
using WebApi.Auth.Entity;
using WebApi.Data.Dtos.Auth;
using WebApi.Data.Dtos.Chat;
using WebApi.Data.Dtos.Rating;
using WebApi.Data.Dtos.Trip;
using WebApi.Data.Entities;

namespace WebApi.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserTrip, UserTripDto>();

        CreateMap<Trip, TripDto>()
            .ForCtorParam("DriverRating", opt => opt.MapFrom(t => t.User.AverageRating));
        CreateMap<CreateTripDto, Trip>();
        CreateMap<Trip, ViewTripDto>();

        CreateMap<Rating, RatingDto>()
            .ForCtorParam(nameof(RatingDto.RatedUser), opt => opt.MapFrom(r => r.RatedUser))
            .ForCtorParam(nameof(RatingDto.User), opt => opt.MapFrom(r => r.RatingUser));
        CreateMap<CreateRatingDto, Rating>();

        CreateMap<Message, MessageDto>();
    }
}
