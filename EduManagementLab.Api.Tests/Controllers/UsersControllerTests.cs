using AutoMapper;
using EduManagementLab.Api.Controllers;
using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using Xunit;
using static EduManagementLab.Api.Controllers.UsersController;

namespace EduManagementLab.Api.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly DbContextOptions<DataContext> _contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("UsersControllerTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        private readonly MapperConfiguration _config = new MapperConfiguration(cfg => cfg.AddProfile<UsersController.UserAutoMapperProfile>());

        private readonly DataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserService _userService;
        private readonly UsersController _usersController;

        DataContext CreateContext() => new DataContext(_contextOptions);
        UnitOfWork CreateUnitOfWork() => new UnitOfWork(_dataContext);
        UserService CreateUserService() => new UserService(_unitOfWork);
        IMapper CreateMapper() => _config.CreateMapper();
        UsersController CreateUsersController() => new UsersController(_userService, CreateMapper());

        public UsersControllerTests()
        {
            _dataContext = CreateContext();

            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _unitOfWork = CreateUnitOfWork();
            _userService = CreateUserService();
            _usersController = CreateUsersController();

            User user1 = new User { Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"), UserName = "DisplayNameOne13531", PasswordHash = _userService.GenerateHashPassword("Test1221"), Displayname = "DisplayNameOne", Email = "EmailOne@Test.com", FirstName = "FirstNameOne", LastName = "LastNameOne" };
            User user2 = new User { Id = Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F"), UserName = "DisplayNameTwo2353", PasswordHash = _userService.GenerateHashPassword("Test7543"), Displayname = "DisplayNameTwo", Email = "EmailTwo@Test.com", FirstName = "FirstNameOne", LastName = "LastNameTwo" };

            _dataContext.AddRange(
                user1, user2
                );

            _dataContext.SaveChanges();

            var mapper = _config.CreateMapper();
        }

        [Fact]
        public void GetUsers_ReturnsOkObjectResult()
        {
            var response = _usersController.GetUsers();

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetUsers_ReturnsCorrectUsers()
        {
            var response = _usersController.GetUsers();
            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<List<UserDto>>(okObjectResult.Value);
            Assert.Equal(2, items.Count);
            Assert.Equal("DisplayNameOne", items[0].DisplayName);
        }

        [Fact]
        public void GetUser_ExistingGuidPassed_ReturnsOkObjectResult()
        {
            var response = _usersController.GetUser(Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"));

            Assert.IsType<OkObjectResult>(response.Result as OkObjectResult);
        }

        [Fact]
        public void GetUser_ExistingGuidPassed_ReturnsCorrectItem()
        {
            var response = _usersController.GetUser(Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"));
            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<UserDto>(okObjectResult.Value);
            Assert.Equal("DisplayNameOne", items.DisplayName);
        }

        [Fact]
        public void GetUser_UnknownGuidPassed_ReturnsNotFoundResult()
        {
            var response = _usersController.GetUser(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void AddUsers_ValidObjectPassed_ReturnsCreatedAtActionResult()
        {
            var testCreateUserRequest = new UsersController.CreateUserRequest()
            {
                Username = "DisplayNameTest",
                PasswordHash = _userService.GenerateHashPassword("Test5323"),
                DisplayName = "DisplayNameTest",
                FirstName = "FirstNameTest",
                LastName = "LastNameTest",
                Email = "EmailTest@test.com"
            };

            var response = _usersController.AddUser(testCreateUserRequest);

            Assert.IsType<CreatedAtActionResult>(response.Result);
        }

        [Fact]
        public void AddUser_ValidObjectPassed_ReturnsResponseHasCreatedItem()
        {
            var testCreateUserRequest = new UsersController.CreateUserRequest()
            {
                Username = "DisplayNameTest",
                PasswordHash = _userService.GenerateHashPassword("Test5323"),
                DisplayName = "DisplayNameTest",
                FirstName = "FirstNameTest",
                LastName = "LastNameTest",
                Email = "EmailTest@test.com"
            };

            var response = _usersController.AddUser(testCreateUserRequest);
            var createdAtActionResult = response.Result as CreatedAtActionResult;

            var item = createdAtActionResult.Value as UserDto;

            Assert.IsType<UserDto>(item);
            Assert.Equal("DisplayNameTest", item.DisplayName);
        }

        [Fact]
        public void UpdateUserName_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testUpdateUserNameRequest = new UsersController.UpdateUserNameRequest()
            {
                Id = Guid.NewGuid(),
                DisplayName = "DisplayNameChanged",
                FirstName = "FirstNameChanged",
                LastName = "LastNameChanged",
            };

            var response = _usersController.UpdateName(testUpdateUserNameRequest);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void UpdateUserName_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testUpdateUserNameRequest = new UsersController.UpdateUserNameRequest()
            {
                Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"),
                DisplayName = "DisplayNameChanged",
                FirstName = "FirstNameChanged",
                LastName = "LastNameChanged",
            };

            var response = _usersController.UpdateName(testUpdateUserNameRequest);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void UpdateUserName_ExistingObjectIdPassed_ReturnsUpdatedItem()
        {
            var testUpdateUserNameRequest = new UsersController.UpdateUserNameRequest()
            {
                Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"),
                DisplayName = "DisplayNameChanged",
                FirstName = "FirstNameChanged",
                LastName = "LastNameChanged",
            };

            var response = _usersController.UpdateName(testUpdateUserNameRequest);

            var okObjectResult = response.Result as OkObjectResult;

            var item = okObjectResult.Value as UserDto;

            Assert.IsType<UserDto>(item);
            Assert.Equal("DisplayNameChanged", item.DisplayName);
        }

        [Fact]
        public void UpdateUserEmail_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testUpdateUserEmailRequest = new UsersController.UpdateUserEmailRequest()
            {
                Id = Guid.NewGuid(),
                Email = "EmailChanged@test.com"
            };

            var response = _usersController.UpdateEmail(testUpdateUserEmailRequest);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void UpdateUserEmail_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testUpdateUserEmailRequest = new UsersController.UpdateUserEmailRequest()
            {
                Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"),
                Email = "EmailChanged@test.com"
            };

            var response = _usersController.UpdateEmail(testUpdateUserEmailRequest);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void UpdateUserEmail_ExistingObjectIdPassed_ReturnsUpdatedItem()
        {
            var testUpdateUserEmailRequest = new UsersController.UpdateUserEmailRequest()
            {
                Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"),
                Email = "EmailChanged@test.com"
            };

            var response = _usersController.UpdateEmail(testUpdateUserEmailRequest);

            var okObjectResult = response.Result as OkObjectResult;

            var item = okObjectResult.Value as UserDto;

            Assert.IsType<UserDto>(item);
            Assert.Equal("EmailChanged@test.com", item.Email);
        }

        [Fact]
        public void DeleteUser_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testUserId = Guid.NewGuid();

            var response = _usersController.DeleteUser(testUserId);

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void DeleteUser_ExistingObjectIdPassed_ReturnsOkResult()
        {
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _usersController.DeleteUser(testUserId);

            Assert.IsType<OkResult>(response);
        }
    }
}
