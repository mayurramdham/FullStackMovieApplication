using AutoMapper;
using Domain.Entity;
using Domain.Model;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MovieDto, Movie>();
        CreateMap<UpdateMovieDto, Movie>();
    }
}
