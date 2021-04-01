using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper mapper;
        private readonly IFileStorage storage;
        private readonly string CONTAINER = "movies";

        public MoviesController(ApplicationDBContext context, IMapper mapper, IFileStorage storage)
        {
            _context = context;
            this.mapper = mapper;
            this.storage = storage;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMovies()
        {
            var movies = await _context.Movies.ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDTO>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return mapper.Map<MovieDTO>(movie);
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, [FromForm] MovieCreateDTO update)
        {
            var movieDB = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movieDB == null)
            {
                return NotFound();
            }
            movieDB = mapper.Map(update, movieDB);

            if (update.ImageFile != null)
            {
                using var memoryStream = new MemoryStream();
                await update.ImageFile.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(update.ImageFile.FileName);
                movieDB.Poster = await storage.EditFile(content, extension, CONTAINER, movieDB.Poster, update.ImageFile.ContentType);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Movies
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie([FromForm] MovieCreateDTO create)
        {
            var movie = mapper.Map<Movie>(create);

            if (create.ImageFile != null)
            {
                using var memoryStream = new MemoryStream();
                await create.ImageFile.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(create.ImageFile.FileName);
                movie.Poster = await storage.SaveFile(content, extension, CONTAINER, create.ImageFile.ContentType);
            }
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return CreatedAtAction("GetMovie", new { id = movie.Id }, movieDTO);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entity = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            var entityDTO = mapper.Map<MoviePatchDTO>(entity);
            patchDocument.ApplyTo(entityDTO, ModelState);
            var isValid = TryValidateModel(entityDTO);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(entityDTO, entity);
            await _context.SaveChangesAsync();
            return NoContent();

        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return movie;
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
