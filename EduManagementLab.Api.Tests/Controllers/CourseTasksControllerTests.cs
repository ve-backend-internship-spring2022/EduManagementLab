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
using static EduManagementLab.Api.Controllers.CourseTasksController;

namespace EduManagementLab.Api.Tests.Controllers
{
    public class CourseTasksControllerTests
    {
        private readonly DbContextOptions<DataContext> _contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("CourseTasksControllerTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        private readonly MapperConfiguration _config = new MapperConfiguration(cfg => cfg.AddProfile<CourseTaskAutoMapperProfile>());

        private readonly DataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        private readonly CourseTaskService _courseTaskService;
        private readonly CourseTasksController _courseTasksController;

        DataContext CreateContext() => new DataContext(_contextOptions);
        UnitOfWork CreateUnitOfWork() => new UnitOfWork(_dataContext);
        CourseService CreateCourseService() => new CourseService(_unitOfWork);
        UserService CreateUserService() => new UserService(_unitOfWork);
        IMapper CreateMapper() => _config.CreateMapper();
        CourseTaskService CreateCourseTaskService() => new CourseTaskService(_unitOfWork);
        CourseTasksController CreateCourseTasksController() => new CourseTasksController(_courseTaskService, _courseService, CreateMapper());

        public CourseTasksControllerTests()
        {
            _dataContext = CreateContext();

            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _unitOfWork = CreateUnitOfWork();
            _courseService = CreateCourseService();
            _userService = CreateUserService();
            _courseTaskService = CreateCourseTaskService();
            _courseTasksController = CreateCourseTasksController();

            Course course1 = new Course { Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), Code = "AAA", Name = "CourseNameOne", Description = "CourseDescriptionOne", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };
            Course course2 = new Course { Id = Guid.Parse("4A0E4335-08E0-45C0-8A97-9791CE81E73D"), Code = "BBB", Name = "CourseNameTwo", Description = "CourseDescriptionTwo", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };

            User user1 = new User { Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"), UserName = "DisplaynameOne5233", PasswordHash = _userService.GenerateHashPassword("Test3513"), Displayname = "DisplaynameOne", Email = "EmailOne@Test.com", FirstName = "FirstNameOne", LastName = "LastNameOne" };
            User user2 = new User { Id = Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F"), UserName = "DisplaynameTwo4364", PasswordHash = _userService.GenerateHashPassword("Test3513"), Displayname = "DisplaynameTwo", Email = "EmailTwo@Test.com", FirstName = "FirstNameOne", LastName = "LastNameTwo" };

            Course.Membership membership1 = new Course.Membership { Id = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED"), Course = course1, CourseId = course1.Id, User = user1, UserId = user1.Id, EnrolledDate = DateTime.MinValue };
            Course.Membership membership2 = new Course.Membership { Id = Guid.Parse("5901AAC8-445A-4A7B-984A-F9C0916CA2A6"), Course = course2, CourseId = course2.Id, User = user2, UserId = user2.Id, EnrolledDate = DateTime.MinValue };
            
            
            CourseTask courseTask1 = new CourseTask { Id = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"), Name = "Assignment1", Description = "", LastUpdate = DateTime.MinValue };
            CourseTask courseTask2 = new CourseTask { Id = Guid.Parse("E6645299-47BB-4D5D-B3FA-316BC51F09F5"), Name = "Assignment2", Description = "This is Assignment 2", LastUpdate = DateTime.MinValue };

            course1.CourseTasks.Add(courseTask1);
            course2.CourseTasks.Add(courseTask2);

            CourseTask.Result courseTaskResult1 = new CourseTask.Result { Id = Guid.Parse("116E1A55-C816-425E-B385-FB476586DB7F"), Membership = membership1, MembershipId = membership1.Id, CourseTaskId = courseTask1.Id, CourseTask = courseTask1, Score = 50.00M, LastUpdated = DateTime.MinValue };
            CourseTask.Result courseTaskResult2 = new CourseTask.Result { Id = Guid.Parse("31415732-6F85-4A9C-A456-0336F169DD31"), Membership = membership2, MembershipId = membership2.Id, CourseTaskId = courseTask2.Id, CourseTask = courseTask2, Score = 80.00M, LastUpdated = DateTime.MinValue };

            _dataContext.AddRange(
                course1, course2,
                user1, user2,
                membership1, membership2,
                courseTask1, courseTask2,
                courseTaskResult1, courseTaskResult2);

            _dataContext.SaveChanges();

            var mapper = _config.CreateMapper();
        }

        [Fact]
        public void GetCourseTasks_ReturnsOkObjectResult()
        {
            var response = _courseTasksController.GetCourseTasks(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"));

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetCourseTasks_ReturnsCorrectCourseTasks()
        {
            var response = _courseTasksController.GetCourseTasks(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"));
            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<List<CourseTaskDto>>(okObjectResult.Value);
            Assert.Equal(1, items.Count);
            Assert.Equal("Assignment1", items[0].Name);
        }

        [Fact]
        public void GetCourseTask_ExistingGuidPassed_ReturnsOkObjectResult()
        {
            var response = _courseTasksController.GetCourseTask(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));

            Assert.IsType<OkObjectResult>(response.Result as OkObjectResult);
        }

        [Fact]
        public void GetCourseTask_ExistingGuidPassed_ReturnsCorrectItem()
        {
            var response = _courseTasksController.GetCourseTask(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));
            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<CourseTaskDto>(okObjectResult.Value);
            Assert.Equal("Assignment1", items.Name);
        }

        [Fact]
        public void GetCourseTask_UnknownGuidPassed_ReturnsNotFoundResult()
        {
            var response = _courseTasksController.GetCourseTask(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void AddCourseTask_ValidObjectPassed_ReturnsCreatedAtActionResult()
        {
            var courseId = Guid.Parse("4A0E4335-08E0-45C0-8A97-9791CE81E73D");
            string CourseTaskName = "Assignment1";
            string CourseTaskDescription = "";

            var response = _courseTasksController.AddCourseTask(courseId, CourseTaskName, CourseTaskDescription);

            Assert.IsType<CreatedAtActionResult>(response.Result);
        }

        [Fact]
        public void AddCourseTask_ValidObjectPassed_ReturnsResponseHasCreatedItem()
        {
            var courseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            string CourseTaskName = "Assignment12";
            string CourseTaskDescription = "TestIQ2";

            var response = _courseTasksController.AddCourseTask(courseId, CourseTaskName, CourseTaskDescription);
            var createdAtActionResult = response.Result as CreatedAtActionResult;

            var item = createdAtActionResult.Value as CourseTaskDto;

            Assert.IsType<CourseTaskDto>(item);
            Assert.Equal("Assignment12", item.Name);
        }

        [Fact]
        public void UpdateCourseTaskInfo_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var courseTaskId = Guid.Parse("A9C797B6-F1D5-43FE-BE7E-7D1E3AAE45C6");
            string CourseTaskName = "Assignment98";
            string CourseTaskDescription = "Test798";

            var response = _courseTasksController.UpdateCourseTaskInfo(courseTaskId, CourseTaskName, CourseTaskDescription);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseTaskInfo_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testUpdateCourseInfoRequest = new UpdateCourseTaskInfoRequest()
            {
                Id = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"),
                Name = "ChangedAssignmentName",
                Description = "ChangedDescription",
                LastUpdated = DateTime.Now
            };

            var response = _courseTasksController.UpdateCourseTaskInfo(testUpdateCourseInfoRequest.Id, testUpdateCourseInfoRequest.Name, testUpdateCourseInfoRequest.Description);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseTaskInfo_ExistingObjectIdPassed_ReturnsUpdatedItem()
        {
            var testUpdateCourseInfoRequest = new UpdateCourseTaskInfoRequest()
            {
                Id = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"),
                Name = "ChangedAssignmentName",
                Description = "ChangedDescription",
                LastUpdated = DateTime.Now
            };

            var response = _courseTasksController.UpdateCourseTaskInfo(testUpdateCourseInfoRequest.Id, testUpdateCourseInfoRequest.Name, testUpdateCourseInfoRequest.Description);

            var okObjectResult = response.Result as OkObjectResult;

            var item = okObjectResult.Value as CourseTaskDto;

            Assert.IsType<CourseTaskDto>(item);
            Assert.Equal("ChangedAssignmentName", item.Name);
        }

        [Fact]
        public void DeleteCourseTask_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testCourseTaskId = Guid.NewGuid();

            var response = _courseTasksController.DeleteCourseTask(testCourseTaskId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void DeleteCourseTask_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var response = _courseTasksController.DeleteCourseTask(testCourseTaskId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void AddCourseTaskResult_ValidObjectPassed_ReturnsCreatedAtActionResult()
        {
            var testCourseTaskId = Guid.Parse("E6645299-47BB-4D5D-B3FA-316BC51F09F5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testScore = 60.00M;

            var response = _courseTasksController.AddCourseTaskResult(testCourseTaskId, testMemberId, testScore);

            Assert.IsType<CreatedAtActionResult>(response.Result);
        }

        [Fact]
        public void AddCourseTaskResult_ValidObjectPassed_ReturnsResponseHasCreatedItem()
        {
            var testCourseTaskId = Guid.Parse("E6645299-47BB-4D5D-B3FA-316BC51F09F5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testScore = 50.00M;

            var response = _courseTasksController.AddCourseTaskResult(testCourseTaskId, testMemberId, testScore);

            var createdAtActionResult = response.Result as CreatedAtActionResult;

            var item = createdAtActionResult.Value as CourseTaskDto.ResultDto;

            Assert.IsType<CourseTaskDto.ResultDto>(item);
            Assert.Equal(testScore, item.Score);
        }

        [Fact]
        public void GetCourseTaskResults_UnknownObjectPassed_ReturnsNotFoundResult()
        {
            var testCourseTaskId = Guid.NewGuid();

            var response = _courseTasksController.GetCourseTaskResults(testCourseTaskId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetCourseTaskResults_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var response = _courseTasksController.GetCourseTaskResults(testCourseTaskId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetCourseTaskResults_ValidObjectPassed_ReturnsCorrectCourseTaskResult()
        {

            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var response = _courseTasksController.GetCourseTaskResults(testCourseTaskId);

            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<List<CourseTaskDto.ResultDto>>(okObjectResult.Value);
            Assert.Equal(1, items.Count);
            Assert.Equal(testCourseTaskId, items[0].CourseTaskId);
        }

        [Fact]
        public void GetCourseTaskResult_UnknownCourseIdPassed_ReturnsNotFoundResult()
        {
            var testCourseTaskId = Guid.NewGuid();
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _courseTasksController.GetCourseTaskResult(testCourseTaskId, testUserId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetCourseTaskResult_UnknownUserIdPassed_ReturnsNotFoundResult()
        {
            var testCourseTaskId = Guid.Parse("116E1A55-C816-425E-B385-FB476586DB7F");
            var testUserId = Guid.NewGuid();

            var response = _courseTasksController.GetCourseTaskResult(testCourseTaskId, testUserId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetCourseTaskResult_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");

            var response = _courseTasksController.GetCourseTaskResult(testCourseTaskId, testMemberId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetCourseTaskResult_ValidObjectPassed_ReturnsCorrectCourseTaskResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");

            var response = _courseTasksController.GetCourseTaskResult(testCourseTaskId, testMemberId);

            var okObjectResult = response.Result as OkObjectResult;

            var item = Assert.IsType<CourseTaskDto.ResultDto>(okObjectResult.Value);
            Assert.Equal(testCourseTaskId, item.CourseTaskId);
        }

        [Fact]
        public void UpdateCourseTaskResult_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testCourseTaskId = Guid.Parse("C05174CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA420-62EC-4A90-BD85-647CD15159ED");

            var response = _courseTasksController.UpdateCourseTaskResult(testCourseTaskId, testMemberId, 90.00M);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseTaskResult_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testUpdateScore = 50.00M;

            var response = _courseTasksController.UpdateCourseTaskResult(testCourseTaskId, testMemberId, testUpdateScore);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseTaskResult_ExistingObjectIdPassed_ReturnsCorrectUpdatedItem()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testUpdateScore = 100.00M;

            var response = _courseTasksController.UpdateCourseTaskResult(testCourseTaskId, testMemberId, testUpdateScore);

            var okObjectResult = response.Result as OkObjectResult;

            var item = okObjectResult.Value as CourseTaskDto.ResultDto;

            Assert.IsType<CourseTaskDto.ResultDto>(item);
            Assert.Equal(testUpdateScore, item.Score);
        }

        [Fact]
        public void DeleteCourseTaskResult_UnknownMemberIdPassed_ReturnsNotFoundMemberObjectResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.NewGuid();

            var response = _courseTasksController.DeleteCourseTaskResult(testCourseTaskId, testMemberId);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }


        [Fact]
        public void DeleteCourseTaskResult_UnknownCourseTaskIdPassed_ReturnsNotFoundCourseTaskObjectResult()
        {
            var testCourseTaskId = Guid.NewGuid();
            var testMemberId = Guid.Parse("8E7A4A78-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _courseTasksController.DeleteCourseTaskResult(testCourseTaskId, testMemberId);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public void DeleteCourseTask_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var response = _courseTasksController.DeleteCourseTask(testCourseTaskId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void DeleteCourseTaskResult_ValidObjectPassed_ReturnsCorrectResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");

            var response = _courseTasksController.DeleteCourseTaskResult(testCourseTaskId, testMemberId);

            var okObjectResult = response.Result as OkObjectResult;

            var item = Assert.IsType<CourseTaskDto.ResultDto>(okObjectResult.Value);
            Assert.Equal(testCourseTaskId, item.CourseTaskId);
            Assert.Equal(testMemberId, item.MembershipId);
        }
    }
}
