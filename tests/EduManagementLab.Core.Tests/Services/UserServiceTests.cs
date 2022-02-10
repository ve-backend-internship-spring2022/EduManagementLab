using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using EduManagementLab.Core.Entities;

namespace EduManagementLab.Core.Tests.Services
{
    public class UserServiceTests
    {
        private readonly DbContextOptions<DataContext> _contextOptions = new DbContextOptionsBuilder<DataContext>()
               .UseInMemoryDatabase("UserServiceTest")
               .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;

        private readonly DataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserService _userService;
        DataContext CreateContext() => new DataContext(_contextOptions);
        UnitOfWork CreateUnitOfWork() => new UnitOfWork(_dataContext);
        UserService CreateUserService() => new UserService(_unitOfWork);

        public UserServiceTests()
        {

            _dataContext = CreateContext();

            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _dataContext.AddRange(
                 new User { Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"), Displayname = "DisplaynameOne", Email = "EmailOne@Test.com", FirstName = "FirstNameOne", LastName = "LastNameOne" },
                 new User { Id = Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F"), Displayname = "DisplaynameTwo", Email = "EmailTwo@Test.com", FirstName = "FirstNameOne", LastName = "LastNameTwo" }
                );

            _dataContext.SaveChanges();

            _unitOfWork = CreateUnitOfWork();
            _userService = CreateUserService();
        }

        [Fact]
        public void GetUsers_ReturnsCorrectUser()
        {
            var fetchedUsers = _userService.GetUsers();

            Assert.Collection(
                fetchedUsers,
                u => Assert.Equal("DisplaynameOne", u.Displayname),
                u => Assert.Equal("DisplaynameTwo", u.Displayname));
        }

        [Fact]
        public void GetUser_ReturnsCorrectUser()
        {
            var fetchedUser = _userService.GetUser(Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"));

            Assert.Equal("DisplaynameOne", fetchedUser.Displayname);
        }

        [Fact]
        public void CreateUser_ReturnsCorrectUser()
        {
            var createdUser = _userService.CreateUser("DisplaynameThree", "FirstNameThree", "LastNameThree", "EmailThree@Test.com");

            var user = _dataContext.Users.Single(b => b.Displayname == "DisplaynameThree");
            Assert.Equal("DisplaynameThree", user.Displayname);
        }

        [Fact]
        public void UpdateName_ReturnsCorrectUser()
        {
            _userService.UpdateName(Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"), "DisplaynameChanged", "FirstNameChanged", "LastNameChanged");

            var user = _dataContext.Users.Single(b => b.Id == Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"));

            Assert.NotNull(user);
            Assert.Equal("DisplaynameChanged", user.Displayname);
        }

        [Fact]
        public void UpdateEmail_ReturnsCorrectUser()
        {
            _userService.UpdateEmail(Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"), "EmailChanged@Test.com");

            var user = _dataContext.Users.Single(b => b.Id == Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"));

            Assert.NotNull(user);
            Assert.Equal("EmailChanged@Test.com", user.Email);
        }

        [Fact]
        public void DeleteUser_DeletesCorrectUser()
        {
            _userService.DeleteUser(Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"));

            var fetchedUsers = _userService.GetUsers();

            Assert.Single(fetchedUsers);
        }
    }
}
