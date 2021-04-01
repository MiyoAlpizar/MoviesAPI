using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Entities
{
    public class CinemaRoom : IId
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Name { get; set; }

        public Point Location { get; set; }

        public List<MoviesCinemaRooms> MoviesCinemaRooms { get; set; }

    }

    public class MoviesCinemaRooms
    {
        public int MovieId { get; set; }
        public int CinemaRoomId { get; set; }
        public Movie Movie { get; set; }
        public CinemaRoom CinemaRoom { get; set; }

    }
}
