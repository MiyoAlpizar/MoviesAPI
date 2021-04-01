using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Comments { get; set; }
        public int Rate { get; set; }
        public int MovieId { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }

    public class ReviewCreateDTO
    {
        public string Comments { get; set; }
        [Range(1,5)]
        public int Rate { get; set; }
        
    }
}
