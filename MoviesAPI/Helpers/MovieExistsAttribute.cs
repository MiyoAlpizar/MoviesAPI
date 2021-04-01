using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class MovieExistsAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDBContext dBContext;

        public MovieExistsAttribute(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var movieIdObject = context.HttpContext.Request.RouteValues["movieId"];
            if (movieIdObject == null) return;

            var movieId = int.Parse(movieIdObject.ToString());
            var movieExists = await dBContext.Movies.AnyAsync(x => x.Id == movieId);
            if (!movieExists) {
                context.Result = new NotFoundResult();
            } else
            {
                await next();
            }
        }
    }
}
