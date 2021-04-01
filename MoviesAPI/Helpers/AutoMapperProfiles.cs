using AutoMapper;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Gender, GenderDTO>().ReverseMap();
            CreateMap<CreateGenderDTO, Gender>();
            CreateMap<ActorDTO, Actor>().ReverseMap();
            CreateMap<CreateActorDTO, Actor>().ForMember(x => x.Image, options => options.Ignore());
            CreateMap<PatchActorDTO, Actor>().ReverseMap();
            CreateMap<MovieDTO, Movie>().ReverseMap();
            CreateMap<MovieCreateDTO, Movie>().ForMember(x => x.Poster, o => o.Ignore());
            CreateMap<MoviePatchDTO, Movie>().ReverseMap();
        }
    }
}
