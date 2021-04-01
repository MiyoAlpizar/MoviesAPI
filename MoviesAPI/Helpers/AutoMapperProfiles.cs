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
            CreateMap<MovieCreateDTO, Movie>()
                .ForMember(x => x.Poster, o => o.Ignore())
                .ForMember(x => x.MoviesGenders, o => o.MapFrom(MapMoviesGenders))
                .ForMember(x => x.MoviesActors, o => o.MapFrom(MapMoviesActors));
            CreateMap<MoviePatchDTO, Movie>().ReverseMap();

            CreateMap<Movie, MovieDetailsDTO>()
                .ForMember(x => x.Genders, o => o.MapFrom(MapMovieGenders))
                .ForMember(x => x.Actors, o => o.MapFrom(MapMovieActors));

            CreateMap<CinemaRoom, CinemaRoomCreateDTO>().ReverseMap();
            CreateMap<CinemaRoom, CinemaRoomDTO>().ReverseMap();


        }

        private List<ActorMovieDetailDTO> MapMovieActors(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var results = new List<ActorMovieDetailDTO>();
            if (movie.MoviesGenders == null) { return results; }
            foreach (var item in movie.MoviesActors)
            {
                results.Add(new ActorMovieDetailDTO { ActorId = item.ActorId, Name = item.Actor.Name, Character = item.Character  });
            }
            return results;
        }

        private List<GenderDTO> MapMovieGenders(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var results = new List<GenderDTO>();
            if (movie.MoviesGenders == null) { return results; }
            foreach (var item in movie.MoviesGenders)
            {
                results.Add(new GenderDTO { Id = item.GenderId, Name = item.Gender.Name });
            }
            return results;
        }

        private List<MoviesGenders> MapMoviesGenders(MovieCreateDTO movieCreateDTO, Movie movie)
        {
            var results = new List<MoviesGenders>();
            if (movieCreateDTO.GendersIDs  == null) { return results; }
            foreach (var item in movieCreateDTO.GendersIDs)
            {
                results.Add(new MoviesGenders { GenderId = item });
            }
            return results;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreateDTO movieCreateDTO, Movie movie)
        {
            var results = new List<MoviesActors>();
            if (movieCreateDTO.GendersIDs == null) { return results; }
            foreach (var item in movieCreateDTO.Actors)
            {
                results.Add(new MoviesActors { ActorId = item.ActorId, Character = item.CharacterName });
            }
            return results;
        }
    }
}
