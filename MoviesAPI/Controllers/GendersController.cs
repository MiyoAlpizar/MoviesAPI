using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class GendersController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GendersController(ApplicationDBContext context, IMapper mapper) : base (context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenderDTO>>> Get()
        {
            return await Get<Gender, GenderDTO>();
        }

        [HttpGet("{id:int}", Name ="GetGender")]
        public async Task<ActionResult<GenderDTO>> Get(int id)
        {
            return await Get<Gender, GenderDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult<GenderDTO>> Post([FromBody] CreateGenderDTO create)
        {
            return await Post<CreateGenderDTO, Gender, GenderDTO>(create, "GetGender");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateGenderDTO create)
        {
            return await Put<CreateGenderDTO, Gender>(id, create);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Gender>(id);
        }

    }
}
