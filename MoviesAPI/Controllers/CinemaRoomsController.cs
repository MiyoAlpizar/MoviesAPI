using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CinemaRoomsController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly GeometryFactory geometryFactory;

        public CinemaRoomsController(ApplicationDBContext context, IMapper mapper, GeometryFactory geometryFactory) : base(context, mapper)
        {
            this.context = context;
            this.geometryFactory = geometryFactory;
        }

        [HttpGet]
        public async Task<ActionResult<List<CinemaRoomDTO>>> Get()
        {
            return await Get<CinemaRoom, CinemaRoomDTO>();
        }

        [HttpGet("near")]
        public async Task<ActionResult<List<NearCinemaRoomDTO>>> GetNear([FromQuery] NearCinemaRoomFilterDTO filterDTO)
        {

             var userLocation = geometryFactory.CreatePoint(new Coordinate(filterDTO.Longitude, filterDTO.Latitude));
            var rooms = await context.CinemaRooms
               .OrderBy(x => x.Location.Distance(userLocation))
               .Where(x => x.Location.IsWithinDistance(userLocation, filterDTO.DistanceInKm * 1000))
               .Select(x => new NearCinemaRoomDTO
               {
                   Id = x.Id,
                   Name = x.Name,
                   Latitude = x.Location.Y,
                   Longitude = x.Location.X,
                   DistanceInMeters = Math.Round(x.Location.Distance(userLocation))
               }).ToListAsync();
            return rooms;
        }

        [HttpGet("{id}", Name = "GetCinemaRoom")]
        public async Task<ActionResult<CinemaRoomDTO>> Get(int id)
        {
            return await Get<CinemaRoom, CinemaRoomDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CinemaRoomCreateDTO cinemaRoomCreateDTO)
        {
            return await Post<CinemaRoomCreateDTO, CinemaRoom, CinemaRoomDTO>(cinemaRoomCreateDTO, "GetCinemaRoom");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CinemaRoomCreateDTO cinemaRoomCreateDTO)
        {
            return await Put<CinemaRoomCreateDTO, CinemaRoom>(id, cinemaRoomCreateDTO);
        }

    }
}
