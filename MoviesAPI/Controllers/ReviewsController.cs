using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/movies/{movieId:int}/reviews")]
    [ServiceFilter(typeof(MovieExistsAttribute))]
    public class ReviewsController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ReviewsController(ApplicationDBContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<List<ReviewDTO>>> Get(int movieId, [FromQuery] PaginationDTO pagination)
        {
            var queryable = context.Reviews.Include(x => x.User).AsQueryable();
            queryable = queryable.Where(x => x.MovieId == movieId);
            return await Get<Review, ReviewDTO>(pagination, queryable);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int movieId, [FromBody] ReviewCreateDTO createDTO)
        { 
            var userId = HttpContext.GetUserId();

            var reviewExists = await context.Reviews.AnyAsync(x => x.MovieId == movieId && x.UserId == userId);
            if (reviewExists)
            {
                return BadRequest("User has already rate this movie");
            }

            var review = mapper.Map<Review>(createDTO);
            review.MovieId = movieId;
            review.UserId = userId;

            context.Add(review);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{reviewId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int reviewId, [FromBody] ReviewCreateDTO createDTO)
        {
           
            var userId = HttpContext.GetUserId();
            var review = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != userId)
            {
                return BadRequest("User has not permissions to edit this review");
            }
            review = mapper.Map(createDTO, review);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{reviewId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int movieId, int reviewId)
        {
            
            var userId = HttpContext.GetUserId();
            var review = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (review == null)
            {
                return NotFound();
            }
            if (review.UserId != userId)
            {
                return Forbid();
            }

            return await Delete<Review>(reviewId);
        }


    }
}
