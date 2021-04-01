using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
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
       
        public CinemaRoomsController(ApplicationDBContext context, IMapper mapper) : base(context, mapper)
        {
        
        }

        [HttpGet]
        public async Task<ActionResult<List<CinemaRoomDTO>>> Get()
        {
            return await Get<CinemaRoom, CinemaRoomDTO>();
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
