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
    public class GendersController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GendersController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenderDTO>>> Get()
        {
            var genders = await context.Genders.ToListAsync();
            return mapper.Map<List<GenderDTO>>(genders);
        }

        [HttpGet("{id:int}", Name ="GetGender")]
        public async Task<ActionResult<GenderDTO>> Get(int id)
        {
            var entity = await context.Genders.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return NotFound();
            }
            return mapper.Map<GenderDTO>(entity);
        }

        [HttpPost]
        public async Task<ActionResult<GenderDTO>> Post([FromBody] CreateGenderDTO create)
        {
            var entidad = mapper.Map<Gender>(create);
            context.Add(entidad);
            await context.SaveChangesAsync();
            var genderDTO = mapper.Map<GenderDTO>(entidad);
            return new CreatedAtRouteResult("GetGender", new { id = genderDTO.Id }, genderDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateGenderDTO create)
        {
            var entidad = mapper.Map<Gender>(create);
            entidad.Id = id;
            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Genders.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            context.Remove(new Gender { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
