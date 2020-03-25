using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeopleSearchApp.DAL;
using PeopleSearchApp.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System.Linq;
using PeopleSearchApp.Models;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace PeopleSearchAppTests
{
    //Unit Tests for people search app. Contains tests for the controller and the data access layer.
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddGetManyUsers()
        {
            PersonContext testContext = InitalizeTestDBContext();

            PersonController testController = new PersonController(testContext);

            for (int i = 0; i < 100; i++)
            {
                testController.AddPerson(new Person { Birthday = new DateTime(2000, 01, 01), FirstName = "Test" + i, LastName = "User", Interests = "Fishing, skiing" });
            }

            Assert.IsTrue(testController.GetPersons("Test").Count == 100);
            testContext.Dispose();
        }

        [TestMethod]
        public void TestAddValidPerson()
        {
            PersonContext testContext = InitalizeTestDBContext();

            PersonController testController = new PersonController(testContext);

            var result = testController.AddPerson(new Person { Birthday = new DateTime(2000, 01, 01), FirstName = "Test", LastName = "User", Interests = "Fishing, skiing" });
            Assert.IsInstanceOfType(result, typeof(OkResult));

            testContext.Dispose();
        }

        [TestMethod]
        public void TestAddInvalidPerson()
        {
            PersonContext testContext = InitalizeTestDBContext();

            PersonController testController = new PersonController(testContext);

            var result = testController.AddPerson(new Person { Birthday = new DateTime(2000, 01, 01) });

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));

            testContext.Dispose();
        }

        [TestMethod]
        public void TestSearchEmptyString()
        {
            PersonContext testContext = InitalizeTestDBContext();

            PersonController testController = new PersonController(testContext);

            testController.AddPerson(new Person { Birthday = new DateTime(2000, 01, 01), FirstName = "Test", LastName = "User", Interests = "Fishing, skiing" });

            List<Person> result = testController.GetPersons("");

            Assert.IsTrue(result.Count() == 0);

            testContext.Dispose();
        }

        [TestMethod]
        public void TestSearchWithSpaceSeparatedFirstLastNames()
        {
            PersonContext testContext = InitalizeTestDBContext();

            PersonController testController = new PersonController(testContext);

            testController.AddPerson(new Person { Birthday = new DateTime(2000, 01, 01), FirstName = "Space", LastName = "Separated", Interests = "Fishing, skiing" });

            List<Person> result = testController.GetPersons("Space Separated");

            Assert.IsTrue(result.Count() == 1);

            testContext.Dispose();
        }

        [TestMethod]
        public void TestSearchCaseInsensitive()
        {
            PersonContext testContext = InitalizeTestDBContext();

            PersonController testController = new PersonController(testContext);

            testController.AddPerson(new Person { Birthday = new DateTime(2000, 01, 01), FirstName = "CASE", LastName = "INSENSTIVE", Interests = "Fishing, skiing" });

            List<Person> result = testController.GetPersons("CASE");

            Assert.IsTrue(result.Count() == 1);

            testContext.Dispose();
        }

        public static PersonContext InitalizeTestDBContext()
        {
            IServiceProvider serviceProvider;

            var services = new ServiceCollection();

            services.AddEntityFrameworkInMemoryDatabase();
            services.AddDbContext<PersonContext>(opt => opt.UseInMemoryDatabase("Persons"));

            serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<PersonContext>();
        }
    }
}