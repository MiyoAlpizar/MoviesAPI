using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class CinemaRoomDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CinemaRoomCreateDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
    }
}
