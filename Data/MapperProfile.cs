using AutoMapper;
using WebApi.Data.Dtos.Rating;
using WebApi.Data.Dtos.Trip;
using WebApi.Data.Entities;

namespace WebApi.Data;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Trip, TripDto>();
        CreateMap<CreateTripDto, Trip>();

        CreateMap<Rating, RatingDto>();
        CreateMap<CreateRatingDto, Rating>();
    }
}
