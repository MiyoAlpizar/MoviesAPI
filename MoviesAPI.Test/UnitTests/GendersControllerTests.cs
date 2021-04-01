using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Test.UnitTests
{
    [TestClass]
    public class GendersControllerTests: TestBase
    {
        [TestMethod]
        public async Task GetAllGenders()
        {
            //Getting Ready
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigureAutoMapper();

            context.Genders.Add(new Entities.Gender { Name = "Drama" });
            context.Genders.Add(new Entities.Gender { Name = "Comedy" });
            context.Genders.Add(new Entities.Gender { Name = "Fantasy" });
            context.Genders.Add(new Entities.Gender { Name = "Accion" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            
            //Test
            var controller = new GendersController(context2, mapper);
            var response = await controller.Get();

            //Verify
            var genders = response.Value;
            Assert.AreEqual(4, genders.Count);
        }


        [TestMethod]
        public async Task GetGenderPerIdNoExistent()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigureAutoMapper();

            var controller = new GendersController(context, mapper);
            var response = await controller.Get(1);

            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);

        }

        [TestMethod]
        public async Task GetGenderPerIdExistent()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigureAutoMapper();

            context.Genders.Add(new Entities.Gender { Name = "Drama" });
            context.Genders.Add(new Entities.Gender { Name = "Comedy" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            var genderID = 2;
            var controller = new GendersController(context2, mapper);
            var response = await controller.Get(genderID);

            var result = response.Value;

            Assert.AreEqual(genderID, result.Id);

        }

        [TestMethod]
        public async Task CreateGender()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigureAutoMapper();
            var newGender = new CreateGenderDTO { Name = "Comedy" };
            var controller = new GendersController(context, mapper);
            var response = await controller.Post(newGender);
            var result = response.Result as CreatedAtRouteResult;
            Assert.IsNotNull(result);

            var context2 = BuildContext(nameDB);
            var count = await context2.Genders.CountAsync();
            Assert.AreEqual(1, count);

        }

        [TestMethod]
        public async Task UpdateGender()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigureAutoMapper();
            var newGender = new Gender { Name = "Comedy" };
            context.Genders.Add(newGender);
            await context.SaveChangesAsync();


            var context2 = BuildContext(nameDB);
            var controller = new GendersController(context2, mapper);

            
            var updateGender = new CreateGenderDTO { Name = "Drama" };

            var response2 = await controller.Put(1, updateGender);
            var result2 = response2 as StatusCodeResult;
            
            Assert.AreEqual(204, result2.StatusCode);

            var context3 = BuildContext(nameDB);
            var exists = await context3.Genders.AnyAsync(x => x.Name == "Drama");

            Assert.IsTrue(exists);

        }

        [TestMethod]
        public async Task TryDeleteGenderNoExists()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigureAutoMapper();

            var controller = new GendersController(context, mapper);
            var response = await controller.Delete(1);
            var result = response as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);

        }

        [TestMethod]
        public async Task DeleteGenderExists()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigureAutoMapper();
            var newGender = new Gender { Name = "Comedy" };
            context.Genders.Add(newGender);
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            var count = await context2.Genders.CountAsync();
            Assert.AreEqual(1, count);

            var context3 = BuildContext(nameDB);
            var controller = new GendersController(context3, mapper);
            await controller.Delete(1);

            var context4 = BuildContext(nameDB);
            var count2 = await context4.Genders.CountAsync();
            Assert.AreEqual(0, count2);
        }

    }
}
