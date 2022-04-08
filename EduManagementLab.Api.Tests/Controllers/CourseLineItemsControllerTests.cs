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
using static EduManagementLab.Api.Controllers.CourseLineItemsController;

namespace EduManagementLab.Api.Tests.Controllers
{
    public class CourseLineItemsControllerTests
    {
        private readonly DbContextOptions<DataContext> _contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("CourseLineItemsControllerTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        private readonly MapperConfiguration _config = new MapperConfiguration(cfg => cfg.AddProfile<CourseLineItemAutoMapperProfile>());

        private readonly DataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        private readonly CourseLineItemService _courseLineItemService;
        private readonly CourseLineItemsController _courseLineItemsController;

        DataContext CreateContext() => new DataContext(_contextOptions);
        UnitOfWork CreateUnitOfWork() => new UnitOfWork(_dataContext);
        CourseService CreateCourseService() => new CourseService(_unitOfWork);
        UserService CreateUserService() => new UserService(_unitOfWork);
        IMapper CreateMapper() => _config.CreateMapper();
        CourseLineItemService CreateCourseLineItemService() => new CourseLineItemService(_unitOfWork);
        CourseLineItemsController CreateCourseLineItemsController() => new CourseLineItemsController(_courseLineItemService, _courseService, CreateMapper());

        public CourseLineItemsControllerTests()
        {
            _dataContext = CreateContext();

            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _unitOfWork = CreateUnitOfWork();
            _courseService = CreateCourseService();
            _userService = CreateUserService();
            _courseLineItemService = CreateCourseLineItemService();
            _courseLineItemsController = CreateCourseLineItemsController();

            Course course1 = new Course { Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), Code = "AAA", Name = "CourseNameOne", Description = "CourseDescriptionOne", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };
            Course course2 = new Course { Id = Guid.Parse("4A0E4335-08E0-45C0-8A97-9791CE81E73D"), Code = "BBB", Name = "CourseNameTwo", Description = "CourseDescriptionTwo", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };

            User user1 = new User { Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"), UserName = "DisplaynameOne5233", PasswordHash = _userService.GenerateHashPassword("Test3513"), Displayname = "DisplaynameOne", Email = "EmailOne@Test.com", FirstName = "FirstNameOne", LastName = "LastNameOne" };
            User user2 = new User { Id = Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F"), UserName = "DisplaynameTwo4364", PasswordHash = _userService.GenerateHashPassword("Test3513"), Displayname = "DisplaynameTwo", Email = "EmailTwo@Test.com", FirstName = "FirstNameOne", LastName = "LastNameTwo" };

            Course.Membership membership1 = new Course.Membership { Id = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED"), Course = course1, CourseId = course1.Id, User = user1, UserId = user1.Id, EnrolledDate = DateTime.MinValue };
            Course.Membership membership2 = new Course.Membership { Id = Guid.Parse("5901AAC8-445A-4A7B-984A-F9C0916CA2A6"), Course = course2, CourseId = course2.Id, User = user2, UserId = user2.Id, EnrolledDate = DateTime.MinValue };
            
            
            CourseLineItem courseLineItem1 = new CourseLineItem { Id = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"), Name = "Assignment1", Description = "", LastUpdate = DateTime.MinValue };
            CourseLineItem courseLineItem2 = new CourseLineItem { Id = Guid.Parse("E6645299-47BB-4D5D-B3FA-316BC51F09F5"), Name = "Assignment2", Description = "This is Assignment 2", LastUpdate = DateTime.MinValue };

            course1.CourseLineItems.Add(courseLineItem1);
            course2.CourseLineItems.Add(courseLineItem2);

            CourseLineItem.Result courseLineItemResult1 = new CourseLineItem.Result { Id = Guid.Parse("116E1A55-C816-425E-B385-FB476586DB7F"), Membership = membership1, MembershipId = membership1.Id, CourseLineItemId = courseLineItem1.Id, CourseLineItem = courseLineItem1, Score = 50.00M, LastUpdated = DateTime.MinValue };
            CourseLineItem.Result courseLineItemResult2 = new CourseLineItem.Result { Id = Guid.Parse("31415732-6F85-4A9C-A456-0336F169DD31"), Membership = membership2, MembershipId = membership2.Id, CourseLineItemId = courseLineItem2.Id, CourseLineItem = courseLineItem2, Score = 80.00M, LastUpdated = DateTime.MinValue };

            _dataContext.AddRange(
                course1, course2,
                user1, user2,
                membership1, membership2,
                courseLineItem1, courseLineItem2,
                courseLineItemResult1, courseLineItemResult2);

            _dataContext.SaveChanges();

            var mapper = _config.CreateMapper();
        }

        [Fact]
        public void GetCourseLineItems_ReturnsOkObjectResult()
        {
            var response = _courseLineItemsController.GetCourseLineItems(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"));

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetCourseLineItems_ReturnsCorrectCourseLineItems()
        {
            var response = _courseLineItemsController.GetCourseLineItems(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"));
            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<List<CourseLineItemDto>>(okObjectResult.Value);
            Assert.Equal(1, items.Count);
            Assert.Equal("Assignment1", items[0].Name);
        }

        [Fact]
        public void GetCourseLineItem_ExistingGuidPassed_ReturnsOkObjectResult()
        {
            var response = _courseLineItemsController.GetCourseLineItem(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));

            Assert.IsType<OkObjectResult>(response.Result as OkObjectResult);
        }

        [Fact]
        public void GetCourseLineItem_ExistingGuidPassed_ReturnsCorrectItem()
        {
            var response = _courseLineItemsController.GetCourseLineItem(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));
            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<CourseLineItemDto>(okObjectResult.Value);
            Assert.Equal("Assignment1", items.Name);
        }

        [Fact]
        public void GetCourseLineItem_UnknownGuidPassed_ReturnsNotFoundResult()
        {
            var response = _courseLineItemsController.GetCourseLineItem(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void AddCourseLineItem_ValidObjectPassed_ReturnsCreatedAtActionResult()
        {
            var courseId = Guid.Parse("4A0E4335-08E0-45C0-8A97-9791CE81E73D");
            string CourseLineItemName = "Assignment1";
            string CourseLineItemDescription = "";

            var response = _courseLineItemsController.AddCourseLineItem(courseId, CourseLineItemName, CourseLineItemDescription);

            Assert.IsType<CreatedAtActionResult>(response.Result);
        }

        [Fact]
        public void AddCourseLineItem_ValidObjectPassed_ReturnsResponseHasCreatedItem()
        {
            var courseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            string CourseLineItemName = "Assignment12";
            string CourseLineItemDescription = "TestIQ2";

            var response = _courseLineItemsController.AddCourseLineItem(courseId, CourseLineItemName, CourseLineItemDescription);
            var createdAtActionResult = response.Result as CreatedAtActionResult;

            var item = createdAtActionResult.Value as CourseLineItemDto;

            Assert.IsType<CourseLineItemDto>(item);
            Assert.Equal("Assignment12", item.Name);
        }

        [Fact]
        public void UpdateCourseLineItemInfo_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var lineItemId = Guid.Parse("A9C797B6-F1D5-43FE-BE7E-7D1E3AAE45C6");
            string CourseLineItemName = "Assignment98";
            string CourseLineItemDescription = "Test798";

            var response = _courseLineItemsController.UpdateCourseLineItemInfo(lineItemId, CourseLineItemName, CourseLineItemDescription);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseLineItemInfo_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testUpdateCourseInfoRequest = new UpdateCourseLineItemInfoRequest()
            {
                Id = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"),
                Name = "ChangedAssignmentName",
                Description = "ChangedDescription",
                LastUpdated = DateTime.Now
            };

            var response = _courseLineItemsController.UpdateCourseLineItemInfo(testUpdateCourseInfoRequest.Id, testUpdateCourseInfoRequest.Name, testUpdateCourseInfoRequest.Description);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseLineItemInfo_ExistingObjectIdPassed_ReturnsUpdatedItem()
        {
            var testUpdateCourseInfoRequest = new UpdateCourseLineItemInfoRequest()
            {
                Id = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"),
                Name = "ChangedAssignmentName",
                Description = "ChangedDescription",
                LastUpdated = DateTime.Now
            };

            var response = _courseLineItemsController.UpdateCourseLineItemInfo(testUpdateCourseInfoRequest.Id, testUpdateCourseInfoRequest.Name, testUpdateCourseInfoRequest.Description);

            var okObjectResult = response.Result as OkObjectResult;

            var item = okObjectResult.Value as CourseLineItemDto;

            Assert.IsType<CourseLineItemDto>(item);
            Assert.Equal("ChangedAssignmentName", item.Name);
        }

        [Fact]
        public void DeleteCourseLineItem_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testCourseLineItemId = Guid.NewGuid();

            var response = _courseLineItemsController.DeleteCourseLineItem(testCourseLineItemId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void DeleteCourseLineItem_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testCourseLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var response = _courseLineItemsController.DeleteCourseLineItem(testCourseLineItemId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void AddLineItemResult_ValidObjectPassed_ReturnsCreatedAtActionResult()
        {
            var testCourseLineItemId = Guid.Parse("E6645299-47BB-4D5D-B3FA-316BC51F09F5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testScore = 60.00M;

            var response = _courseLineItemsController.AddLineItemResult(testCourseLineItemId, testMemberId, testScore);

            Assert.IsType<CreatedAtActionResult>(response.Result);
        }

        [Fact]
        public void AddLineItemResult_ValidObjectPassed_ReturnsResponseHasCreatedItem()
        {
            var testCourseLineItemId = Guid.Parse("E6645299-47BB-4D5D-B3FA-316BC51F09F5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testScore = 50.00M;

            var response = _courseLineItemsController.AddLineItemResult(testCourseLineItemId, testMemberId, testScore);

            var createdAtActionResult = response.Result as CreatedAtActionResult;

            var item = createdAtActionResult.Value as CourseLineItemDto.ResultDto;

            Assert.IsType<CourseLineItemDto.ResultDto>(item);
            Assert.Equal(testScore, item.Score);
        }

        [Fact]
        public void GetLineItemResults_UnknownObjectPassed_ReturnsNotFoundResult()
        {
            var testLineItemResultId = Guid.NewGuid();

            var response = _courseLineItemsController.GetLineItemResults(testLineItemResultId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetLineItemResults_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var response = _courseLineItemsController.GetLineItemResults(testLineItemId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetLineItemResults_ValidObjectPassed_ReturnsCorrectLineItemResult()
        {

            var testLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var response = _courseLineItemsController.GetLineItemResults(testLineItemId);

            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<List<CourseLineItemDto.ResultDto>>(okObjectResult.Value);
            Assert.Equal(1, items.Count);
            Assert.Equal(testLineItemId, items[0].CourseLineItemId);
        }

        [Fact]
        public void GetLineItemResult_UnknownCourseIdPassed_ReturnsNotFoundResult()
        {
            var testLineItemResultId = Guid.NewGuid();
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _courseLineItemsController.GetLineItemResult(testLineItemResultId, testUserId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetLineItemResult_UnknownUserIdPassed_ReturnsNotFoundResult()
        {
            var testLineItemResultId = Guid.Parse("116E1A55-C816-425E-B385-FB476586DB7F");
            var testUserId = Guid.NewGuid();

            var response = _courseLineItemsController.GetLineItemResult(testLineItemResultId, testUserId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetLineItemResult_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");

            var response = _courseLineItemsController.GetLineItemResult(testLineItemId, testMemberId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetLineItemResult_ValidObjectPassed_ReturnsCorrectLineItemResult()
        {
            var testLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");

            var response = _courseLineItemsController.GetLineItemResult(testLineItemId, testMemberId);

            var okObjectResult = response.Result as OkObjectResult;

            var item = Assert.IsType<CourseLineItemDto.ResultDto>(okObjectResult.Value);
            Assert.Equal(testLineItemId, item.CourseLineItemId);
        }

        [Fact]
        public void UpdateLineItemResult_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testLineItemId = Guid.Parse("C05174CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA420-62EC-4A90-BD85-647CD15159ED");

            var response = _courseLineItemsController.UpdateLineItemResult(testLineItemId, testMemberId, 90.00M);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void UpdateLineItemResult_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testUpdateScore = 50.00M;

            var response = _courseLineItemsController.UpdateLineItemResult(testLineItemId, testMemberId, testUpdateScore);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void UpdateLineItemResult_ExistingObjectIdPassed_ReturnsCorrectUpdatedItem()
        {
            var testLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testUpdateScore = 100.00M;

            var response = _courseLineItemsController.UpdateLineItemResult(testLineItemId, testMemberId, testUpdateScore);

            var okObjectResult = response.Result as OkObjectResult;

            var item = okObjectResult.Value as CourseLineItemDto.ResultDto;

            Assert.IsType<CourseLineItemDto.ResultDto>(item);
            Assert.Equal(testUpdateScore, item.Score);
        }

        [Fact]
        public void DeleteLineItemResult_UnknownMemberIdPassed_ReturnsNotFoundMemberObjectResult()
        {
            var testLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.NewGuid();

            var response = _courseLineItemsController.DeleteLineItemResult(testLineItemId, testMemberId);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }


        [Fact]
        public void DeleteLineItemResult_UnknownLineItemIdPassed_ReturnsNotFoundLineItemObjectResult()
        {
            var testLineItemId = Guid.NewGuid();
            var testMemberId = Guid.Parse("8E7A4A78-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _courseLineItemsController.DeleteLineItemResult(testLineItemId, testMemberId);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public void DeleteCourseLineItem_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var response = _courseLineItemsController.DeleteCourseLineItem(testCourseLineItemId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void DeleteCourseLineItemResult_ValidObjectPassed_ReturnsCorrectResult()
        {
            var testCourseLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");

            var response = _courseLineItemsController.DeleteLineItemResult(testCourseLineItemId, testMemberId);

            var okObjectResult = response.Result as OkObjectResult;

            var item = Assert.IsType<CourseLineItemDto.ResultDto>(okObjectResult.Value);
            Assert.Equal(testCourseLineItemId, item.CourseLineItemId);
            Assert.Equal(testMemberId, item.MembershipId);
        }
    }
}
