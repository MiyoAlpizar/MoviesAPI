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
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : CustomBaseController
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper mapper;
        private readonly IFileStorage storage;
        private readonly ILogger<MoviesController> logger;
        private readonly string CONTAINER = "movies";

        public MoviesController(ApplicationDBContext context, IMapper mapper, IFileStorage storage, ILogger<MoviesController> logger): base(context, mapper)
        {
            _context = context;
            this.mapper = mapper;
            this.storage = storage;
            this.logger = logger;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<MoviesIndexDTO>> GetMovies()
        {
            var top = 5;
            var today = DateTime.Today;

            var nextMovies = await _context.Movies
                .OrderBy(x => x.DateIssued)
                .Take(top)
                .ToListAsync();

            var inTheather = await _context.Movies
                .OrderBy(x => x.DateIssued)
                .Take(top)
                .ToListAsync();

            var movies = new MoviesIndexDTO
            {
                InTheathers = mapper.Map<List<MovieDTO>>(inTheather),
                NextMovies = mapper.Map<List<MovieDTO>>(nextMovies)
            };

            return movies;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> FilterMovies([FromQuery] FilterMoviesDTO filter) {
            var moviesQueryable = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filter.Title));
            }

            if (filter.InTheather)
            {
                moviesQueryable = moviesQueryable.Where(x => x.IsInTheater);
            }

            if (filter.Next)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.DateIssued > today);
            }

            if (!string.IsNullOrEmpty(filter.Gender))
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenders
                    .Select(y => y.Gender.Name)
                    .Contains(filter.Gender));
            }

            if (!string.IsNullOrWhiteSpace(filter.FieldOrderBy))
            {
                var asc = filter.AscOrderBy ? "ascending" : "descending";
                try
                {
                    moviesQueryable = moviesQueryable.OrderBy($"{filter.FieldOrderBy} {asc}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }
            }

            await HttpContext.InsertPaginationParams(moviesQueryable, filter.PaginationDTO.QuantityRegistersPerPage);
            var movies = await moviesQueryable.Paginate(filter.PaginationDTO).ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDetailsDTO>> GetMovie(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor)
                .Include(x => x.MoviesGenders).ThenInclude(x => x.Gender)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }
            movie.MoviesActors = movie.MoviesActors.OrderBy(x => x.Order).ToList();

            return mapper.Map<MovieDetailsDTO>(movie);
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, [FromForm] MovieCreateDTO update)
        {
            var movieDB = await _context.Movies
                .Include(x => x.MoviesActors)
                .Include(x => x.MoviesGenders)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (movieDB == null) return NotFound();
            
            movieDB = mapper.Map(update, movieDB);
            SetOrderActors(movieDB);

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

            SetOrderActors(movie);
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return CreatedAtAction("GetMovie", new { id = movie.Id }, movieDTO);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            return await Patch<Movie, MoviePatchDTO>(id, patchDocument);

        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id)
        {
            return await Delete<Movie>(id);
        }

        private void SetOrderActors(Movie movie)
        {
            if (movie.MoviesActors == null) return;

            for (int i = 0; i < movie.MoviesActors.Count; i++)
            {
                movie.MoviesActors[i].Order = i;
            }
        }
    }
}
