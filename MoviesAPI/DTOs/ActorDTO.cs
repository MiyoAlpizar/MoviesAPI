using Microsoft.AspNetCore.Http;
using MoviesAPI.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class ActorDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string Image { get; set; }
    }

    public class CreateActorDTO: PatchActorDTO
    {
        [FileSizeValidation(MaxSize:1)]
        [FileTypeValidation(groupFileType: GroupFileType.Imagen)]
        public IFormFile ImageFile { get; set; }
    }

    public class PatchActorDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
    }
}
