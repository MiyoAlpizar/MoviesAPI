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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class NearCinemaRoomDTO : CinemaRoomDTO
    {
        public double DistanceInMeters { get; set; }
    }


    public class CinemaRoomCreateDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        [Range(-90, 90)]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        public double Longitude { get; set; }
    }

    public class NearCinemaRoomFilterDTO
    {
        [Range(-90, 90)]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        public double Longitude { get; set; }

        private int distanceInKm = 10;
        private readonly int distanceMaxInKm = 2500;
        public int DistanceInKm
        {
            get
            {
                return distanceInKm;
            }
            set
            {
                distanceInKm = (value > distanceMaxInKm ? distanceMaxInKm : value);
            }
        }

    }
}
