using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        public bool IsInTheater { get; set; }
        public DateTime DateIssued { get; set; }
        public string Poster { get; set; }
    }
}
