using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorsController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string CONTAINER = "actors";
        public ActorsController(ApplicationDBContext context, IMapper mapper, IFileStorage fileStorage) : base (context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            return await Get<Actor, ActorDTO>(pagination);
        }

        [HttpGet("{id}", Name ="GetActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            return await Get<Actor, ActorDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult<ActorDTO>> Post([FromForm] CreateActorDTO create)
        {
            var entidad = mapper.Map<Actor>(create);
            if (create.ImageFile != null)
            {
                using var memoryStream = new MemoryStream();
                await create.ImageFile.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(create.ImageFile.FileName);
                entidad.Image = await fileStorage.SaveFile(content, extension, CONTAINER, create.ImageFile.ContentType);
            }
            context.Add(entidad);
            await context.SaveChangesAsync();
            var entity = mapper.Map<ActorDTO>(entidad);
            return new CreatedAtRouteResult("GetActor", new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] CreateActorDTO create)
        {
            var actorDB = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actorDB == null)
            {
                return NotFound();
            }
            actorDB = mapper.Map(create, actorDB);

            if (create.ImageFile != null)
            {
                using var memoryStream = new MemoryStream();
                await create.ImageFile.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(create.ImageFile.FileName);
                actorDB.Image = await fileStorage.EditFile(content, extension, CONTAINER, actorDB.Image, create.ImageFile.ContentType);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PatchActorDTO> patchDocument)
        {
            return await Patch<Actor, PatchActorDTO>(id, patchDocument);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id);
        }
    }
}
