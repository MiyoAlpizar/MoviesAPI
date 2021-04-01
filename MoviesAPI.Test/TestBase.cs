using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Helpers;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Test
{
    public class TestBase
    {
        protected ApplicationDBContext BuildContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(dbName).Options;

            var dbContext = new ApplicationDBContext(options);
            return dbContext;
        }

        protected IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(o =>
            {
               var geometry = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
               o.AddProfile(new AutoMapperProfiles(geometry));
            });
            return config.CreateMapper();
        }
    }
}
