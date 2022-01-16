using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EduManagementLab.Core.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public void CreteUpdateAndGetUser()
        {
            //Not optimal to use a real database. Change to mock db?
            string databaseName = "EduManagementLabDb_Test_CreteUpdateAndGetUser";
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            var dataContext = new DataContext(dbContextOptions);

            dataContext.Database.EnsureDeleted();
            dataContext.Database.EnsureCreated();

            var unitOfWorkCreate = new UnitOfWork(dataContext);
            var userServiceCreate = new UserService(unitOfWorkCreate);
            var createdUser = userServiceCreate.CreateUser("Displayname_initial", "FirstName_initial", "LastName_initial", "Email@initial.test");

            var unitOfWorkUpdate = new UnitOfWork(dataContext);
            var userServiceUpdate = new UserService(unitOfWorkUpdate);
            userServiceUpdate.UpdateName(createdUser.Id, "Displayname_changed", "FirstName_changed", "LastName_changed");
            userServiceUpdate.UpdateEmail(createdUser.Id, "Email@changed.test");

            var unitOfWorkGet = new UnitOfWork(dataContext);
            var userServiceGet = new UserService(unitOfWorkGet);
            var fetchedUser = userServiceGet.GetUser(createdUser.Id);

            Assert.NotNull(fetchedUser);
            Assert.Equal("Displayname_changed", createdUser.Displayname);
            Assert.Equal("FirstName_changed", createdUser.FirstName);
            Assert.Equal("LastName_changed", createdUser.LastName);
            Assert.Equal("Email@changed.test", createdUser.Email);

            dataContext.Database.EnsureDeleted();
        }
    }
}
