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
using static EduManagementLab.Api.Controllers.CoursesController;

namespace EduManagementLab.Api.Tests.Controllers
{
    public class CoursesControllerTests
    {
        private readonly DbContextOptions<DataContext> _contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("CourseServiceTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        private readonly MapperConfiguration _config = new MapperConfiguration(cfg => cfg.AddProfile<CoursesController.UserAutoMapperProfile>());

        private readonly DataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        private readonly CoursesController _coursesController;
        
        DataContext CreateContext() => new DataContext(_contextOptions);
        UnitOfWork CreateUnitOfWork() => new UnitOfWork(_dataContext);
        CourseService CreateCourseService() => new CourseService(_unitOfWork);
        UserService CreateUserService() => new UserService(_unitOfWork);
        IMapper CreateMapper() => _config.CreateMapper();
        CoursesController CreateCoursesController() => new CoursesController(_courseService, _userService, CreateMapper());

        public CoursesControllerTests()
        {
            _dataContext = CreateContext();

            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _dataContext.AddRange(
                new Course { Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), Code = "AAA", Name = "CourseNameOne", Description = "CourseDescriptionOne", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue },
                new Course { Id = Guid.Parse("4A0E4335-08E0-45C0-8A97-9791CE81E73D"), Code = "BBB", Name = "CourseNameTwo", Description = "CourseDescriptionTwo", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue });

            _dataContext.SaveChanges();

            var mapper = _config.CreateMapper();

            _unitOfWork = CreateUnitOfWork();
            _courseService = CreateCourseService();
            _userService = CreateUserService();
            _coursesController = CreateCoursesController();
        }

        [Fact]
        public void GetCourses_ReturnsOkResult()
        {
            var okResult = _coursesController.GetCourses();

            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void GetCourses_ReturnsCorrectCourses()
        {
            var result = _coursesController.GetCourses();
            var okResult = result.Result as OkObjectResult;

            var items = Assert.IsType<List<CourseDto>>(okResult.Value);
            Assert.Equal(2, items.Count);
            Assert.Equal("AAA", items[0].Code);
        }

        [Fact]
        public void GetCourse_ExistingGuidPassed_ReturnsOkResult()
        {
            var result = _coursesController.GetCourse(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"));

            Assert.IsType<OkObjectResult>(result.Result as OkObjectResult);
        }

        [Fact]
        public void GetCourse_ExistingGuidPassed_ReturnsCorrectItem()
        {
            var result = _coursesController.GetCourse(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"));
            var okResult = result.Result as OkObjectResult;

            var items = Assert.IsType<CourseDto>(okResult.Value);
            Assert.Equal("AAA", items.Code);
        }

        [Fact]
        public void GetCourse_UnknownGuidPassed_ReturnsNotFoundResult()
        {
            var notFoundResult = _coursesController.GetCourse(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void AddCourse_ValidObjectPassed_ReturnsCreatedResponse()
        {
            var testCreateCourseRequest = new CoursesController.CreateCourseRequest()
            {
              Code = "TEST",
              Name = "CourseNameTest",
              Description = "CourseDescriptionTest",
              StartDate = DateTime.Now,
              EndDate = DateTime.Now,
            };

            var createdResponse = _coursesController.AddCourse(testCreateCourseRequest);

            Assert.IsType<CreatedAtActionResult>(createdResponse.Result);
        }

        [Fact]
        public void AddCourse_ValidObjectPassed_ReturnedResponseHasCreatedItem()
        {
            var testCreateCourseRequest = new CoursesController.CreateCourseRequest()
            {
                Code = "TEST",
                Name = "CourseNameTest",
                Description = "CourseDescriptionTest",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };

            var createdResponse = _coursesController.AddCourse(testCreateCourseRequest);
            var createdResult = createdResponse.Result as CreatedAtActionResult;

            var item = createdResult.Value as CourseDto;

            Assert.IsType<CourseDto>(item);
            Assert.Equal("CourseNameTest", item.Name);
        }

        [Fact]
        public void UpdateCourseInfo_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testUpdateCourseInfoRequest = new CoursesController.UpdateCourseInfoRequest()
            {
                Id = Guid.Parse("11962A2C-CC68-4EBA-A36E-5809C8741021"),
                Code = "CCC",
                Name = "ChangedName",
                Description = "ChangedDescription",
            };

            var notFoundResult = _coursesController.UpdateCourseInfo(testUpdateCourseInfoRequest);

            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void UpdateCourseInfo_ExistingObjectIdPassed_ReturnsOkResult()
        {
            var testUpdateCourseInfoRequest = new CoursesController.UpdateCourseInfoRequest()
            {
                Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"),
                Code = "CCC",
                Name = "ChangedName",
                Description = "ChangedDescription",
            };

            var updateResponse = _coursesController.UpdateCourseInfo(testUpdateCourseInfoRequest);

            Assert.IsType<OkObjectResult>(updateResponse.Result);
        }

        [Fact]
        public void UpdateCourseInfo_ExistingObjectIdPassed_ReturnsUpdatedItem()
        {
            var testUpdateCourseInfoRequest = new CoursesController.UpdateCourseInfoRequest()
            {
                Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"),
                Code = "CCC",
                Name = "ChangedName",
                Description = "ChangedDescription",
            };

            var updateResponse = _coursesController.UpdateCourseInfo(testUpdateCourseInfoRequest);

            var updateResult = updateResponse.Result as OkObjectResult;

            var item = updateResult.Value as CourseDto;

            Assert.IsType<CourseDto>(item);
            Assert.Equal("ChangedName", item.Name);
        }

        [Fact]
        public void UpdateCoursePeriod_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testUpdateCoursePeriodRequest = new CoursesController.UpdateCoursePeriodRequest()
            {
                Id = Guid.NewGuid(),
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MaxValue,

            };

            var updateResponse = _coursesController.UpdateCoursePeriod(testUpdateCoursePeriodRequest);

            Assert.IsType<NotFoundResult>(updateResponse.Result);
        }

        [Fact]
        public void UpdateCoursePeriod_ExistingObjectIdPassed_ReturnsOkResult()
        {
            var testUpdateCoursePeriodRequest = new CoursesController.UpdateCoursePeriodRequest()
            {
                Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"),
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MaxValue,

            };

            var updateResponse = _coursesController.UpdateCoursePeriod(testUpdateCoursePeriodRequest);

            Assert.IsType<OkObjectResult>(updateResponse.Result);
        }

        [Fact]
        public void UpdateCoursePeriod_ExistingObjectIdPassed_ReturnsUpdatedItem()
        {
            var testUpdateCoursePeriodRequest = new CoursesController.UpdateCoursePeriodRequest()
            {
                Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"),
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MaxValue,

            };

            var updateResponse = _coursesController.UpdateCoursePeriod(testUpdateCoursePeriodRequest);

            var updateResult = updateResponse.Result as OkObjectResult;

            var item = updateResult.Value as CourseDto;

            Assert.IsType<CourseDto>(item);
            Assert.Equal(DateTime.MinValue, item.StartDate);
        }
    }
}
    