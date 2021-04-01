using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Helpers;
using MoviesAPI.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        public bool IsInTheater { get; set; }
        public DateTime DateIssued { get; set; }
        public string Poster { get; set; }
    }

    public class MovieCreateDTO
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        public bool IsInTheater { get; set; }
        public DateTime DateIssued { get; set; }
        
        [FileSizeValidation(MaxSize: 1)]
        [FileTypeValidation(groupFileType: GroupFileType.Imagen)]
        public IFormFile ImageFile { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GendersIDs { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorMovieCreatorDTO>>))]
        public List<ActorMovieCreatorDTO> Actors { get; set; }

    }

    public class MoviePatchDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        public bool IsInTheater { get; set; }
        public DateTime DateIssued { get; set; }
    }


}
