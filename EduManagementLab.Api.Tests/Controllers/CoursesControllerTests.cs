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
                .UseInMemoryDatabase("CoursesControllerTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        private readonly MapperConfiguration _config = new MapperConfiguration(cfg => cfg.AddProfile<CoursesController.CourseAutoMapperProfile>());

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

            Course course1 = new Course { Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), Code = "AAA", Name = "CourseNameOne", Description = "CourseDescriptionOne", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };
            Course course2 = new Course { Id = Guid.Parse("4A0E4335-08E0-45C0-8A97-9791CE81E73D"), Code = "BBB", Name = "CourseNameTwo", Description = "CourseDescriptionTwo", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };

            User user1 = new User { Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"), Displayname = "DisplaynameOne", Email = "EmailOne@Test.com", FirstName = "FirstNameOne", LastName = "LastNameOne"  };
            User user2 = new User { Id = Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F"), Displayname = "DisplaynameTwo", Email = "EmailTwo@Test.com", FirstName = "FirstNameOne", LastName = "LastNameTwo" };

            Course.Membership membership1 = new Course.Membership { Id = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED"), Course = course1, CourseId = course1.Id, User = user1, UserId = user1.Id, EnrolledDate = DateTime.MinValue };
            Course.Membership membership2 = new Course.Membership { Id = Guid.Parse("5901AAC8-445A-4A7B-984A-F9C0916CA2A6"), Course = course2, CourseId = course2.Id, User = user2, UserId = user2.Id, EnrolledDate = DateTime.MinValue };

            _dataContext.AddRange(          
                course1, course2,
                user1, user2,
                membership1, membership2);

            _dataContext.SaveChanges();

            var mapper = _config.CreateMapper();

            _unitOfWork = CreateUnitOfWork();
            _courseService = CreateCourseService();
            _userService = CreateUserService();
            _coursesController = CreateCoursesController();
        }

        [Fact]
        public void GetCourses_ReturnsOkObjectResult()
        {
            var response = _coursesController.GetCourses();

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetCourses_ReturnsCorrectCourses()
        {
            var response = _coursesController.GetCourses();
            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<List<CourseDto>>(okObjectResult.Value);
            Assert.Equal(2, items.Count);
            Assert.Equal("AAA", items[0].Code);
        }

        [Fact]
        public void GetCourse_ExistingGuidPassed_ReturnsOkObjectResult()
        {
            var response = _coursesController.GetCourse(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"));

            Assert.IsType<OkObjectResult>(response.Result as OkObjectResult);
        }

        [Fact]
        public void GetCourse_ExistingGuidPassed_ReturnsCorrectItem()
        {
            var response = _coursesController.GetCourse(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"));
            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<CourseDto>(okObjectResult.Value);
            Assert.Equal("AAA", items.Code);
        }

        [Fact]
        public void GetCourse_UnknownGuidPassed_ReturnsNotFoundResult()
        {
            var response = _coursesController.GetCourse(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void AddCourse_ValidObjectPassed_ReturnsCreatedAtActionResult()
        {
            var testCreateCourseRequest = new CoursesController.CreateCourseRequest()
            {
                Code = "TEST",
                Name = "CourseNameTest",
                Description = "CourseDescriptionTest",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };

            var response = _coursesController.AddCourse(testCreateCourseRequest);

            Assert.IsType<CreatedAtActionResult>(response.Result);
        }

        [Fact]
        public void AddCourse_ValidObjectPassed_ReturnsResponseHasCreatedItem()
        {
            var testCreateCourseRequest = new CoursesController.CreateCourseRequest()
            {
                Code = "TEST",
                Name = "CourseNameTest",
                Description = "CourseDescriptionTest",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };

            var response = _coursesController.AddCourse(testCreateCourseRequest);
            var createdAtActionResult = response.Result as CreatedAtActionResult;

            var item = createdAtActionResult.Value as CourseDto;

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

            var response = _coursesController.UpdateCourseInfo(testUpdateCourseInfoRequest);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseInfo_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testUpdateCourseInfoRequest = new CoursesController.UpdateCourseInfoRequest()
            {
                Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"),
                Code = "CCC",
                Name = "ChangedName",
                Description = "ChangedDescription",
            };

            var response = _coursesController.UpdateCourseInfo(testUpdateCourseInfoRequest);

            Assert.IsType<OkObjectResult>(response.Result);
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

            var response = _coursesController.UpdateCourseInfo(testUpdateCourseInfoRequest);

            var okObjectResult = response.Result as OkObjectResult;

            var item = okObjectResult.Value as CourseDto;

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

            var response = _coursesController.UpdateCoursePeriod(testUpdateCoursePeriodRequest);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void UpdateCoursePeriod_ExistingObjectIdPassed_ReturnsOkObjectResult()
        {
            var testUpdateCoursePeriodRequest = new CoursesController.UpdateCoursePeriodRequest()
            {
                Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"),
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MaxValue,

            };

            var response = _coursesController.UpdateCoursePeriod(testUpdateCoursePeriodRequest);

            Assert.IsType<OkObjectResult>(response.Result);
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

            var response = _coursesController.UpdateCoursePeriod(testUpdateCoursePeriodRequest);

            var okObjectResult = response.Result as OkObjectResult;

            var item = okObjectResult.Value as CourseDto;

            Assert.IsType<CourseDto>(item);
            Assert.Equal(DateTime.MinValue, item.StartDate);
        }

        [Fact]
        public void DeleteCourse_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testCourseId = Guid.NewGuid();

            var response = _coursesController.DeleteCourse(testCourseId);

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void DeleteCourse_ExistingObjectIdPassed_ReturnsOkResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");

            var response = _coursesController.DeleteCourse(testCourseId);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public void AddCourseMembership_ValidObjectPassed_ReturnsCreatedAtActionResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F");
            var testEnrollmentDate = DateTime.MinValue;

            var membershipDto = new MembershipDto() { CourseId = testCourseId, UserId = testUserId, EnrolledDate = testEnrollmentDate };

            var response = _coursesController.AddCourseMembership(membershipDto);

            Assert.IsType<CreatedAtActionResult>(response.Result);
        }

        [Fact]
        public void AddCourseMembership_UnknownCoursePassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.NewGuid();
            var testUserId = Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F");
            var testEnrollmentDate = DateTime.MinValue;

            var membershipDto = new MembershipDto() { CourseId = testCourseId, UserId = testUserId, EnrolledDate = testEnrollmentDate };

            var response = _coursesController.AddCourseMembership(membershipDto);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }


        [Fact]
        public void AddCourseMembership_UnknownUserPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.NewGuid();
            var testEnrollmentDate = DateTime.MinValue;

            var membershipDto = new MembershipDto() { CourseId = testCourseId, UserId = testUserId, EnrolledDate = testEnrollmentDate };

            var response = _coursesController.AddCourseMembership(membershipDto);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public void AddCourseMembership_DuplicateObjectPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");
            var testEnrollmentDate = DateTime.MinValue;

            var membershipDto = new MembershipDto() { CourseId = testCourseId, UserId = testUserId, EnrolledDate = testEnrollmentDate};

            var response = _coursesController.AddCourseMembership(membershipDto);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public void AddCourseMembership_ValidObjectPassed_ReturnsCreatedObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F");
            var testEnrollmentDate = DateTime.MinValue;

            var membershipDto = new MembershipDto() { CourseId = testCourseId, UserId = testUserId, EnrolledDate = testEnrollmentDate };

            var response = _coursesController.AddCourseMembership(membershipDto);

            var okObjectResult = response.Result as CreatedAtActionResult;

            var item = okObjectResult.Value as MembershipDto;

            Assert.IsType<MembershipDto>(item);
            Assert.Equal(item.UserId, Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F"));
        }

        [Fact]
        public void UpdateCourseMembershipEnrolledDate_UnknownCoursePassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.NewGuid();
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");
            var testEnrollmentDate = DateTime.MinValue;

            var membershipDto = new MembershipDto() { CourseId = testCourseId, UserId = testUserId, EnrolledDate = testEnrollmentDate };

            var response = _coursesController.UpdateCourseMembershipEnrolledDate(membershipDto);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseMembershipEnrolledDate_UnknownUserPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.NewGuid();
            var testEnrollmentDate = DateTime.MinValue;

            var membershipDto = new MembershipDto() { CourseId = testCourseId, UserId = testUserId, EnrolledDate = testEnrollmentDate };

            var response = _coursesController.UpdateCourseMembershipEnrolledDate(membershipDto);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseMembershipEnrolledDate_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");
            var testEnrollmentDate = DateTime.MinValue;

            var membershipDto = new MembershipDto() { CourseId = testCourseId, UserId = testUserId, EnrolledDate = testEnrollmentDate };

            var response = _coursesController.UpdateCourseMembershipEnrolledDate(membershipDto);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void UpdateCourseMembershipEnrolledDate_ValidObjectPassed_ReturnsUpdatedObject()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");
            var testEnrollmentDate = DateTime.MinValue;

            var membershipDto = new MembershipDto() { CourseId = testCourseId, UserId = testUserId, EnrolledDate = testEnrollmentDate };

            var response = _coursesController.UpdateCourseMembershipEnrolledDate(membershipDto);

            var okObjectResult = response.Result as OkObjectResult;

            var item = okObjectResult.Value as MembershipDto;

            Assert.IsType<MembershipDto>(item);
            Assert.Equal(item.UserId, Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"));
        }

        [Fact]
        public void GetCourseMemberships_UnknownObjectPassed_ReturnsNotFoundResult()
        {
            var testCourseId = Guid.NewGuid();

            var response = _coursesController.GetCourseMemberships(testCourseId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetCourseMemberships_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");

            var response = _coursesController.GetCourseMemberships(testCourseId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetCourseMemberships_ValidObjectPassed_ReturnsCorrectMemberships()
        {

            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");

            var response = _coursesController.GetCourseMemberships(testCourseId);

            var okObjectResult = response.Result as OkObjectResult;

            var items = Assert.IsType<List<MembershipDto>>(okObjectResult.Value);
            Assert.Equal(1, items.Count);
            Assert.Equal(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), items[0].CourseId);
        }

        [Fact]
        public void GetCourseMembership_UnknownCourseIdPassed_ReturnsNotFoundResult()
        {
            var testCourseId = Guid.NewGuid();
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _coursesController.GetCourseMembership(testCourseId, testUserId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetCourseMembership_UnknownUserIdPassed_ReturnsNotFoundResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.NewGuid();

            var response = _coursesController.GetCourseMembership(testCourseId, testUserId);

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public void GetCourseMembership_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _coursesController.GetCourseMembership(testCourseId, testUserId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void GetCourseMembership_ValidObjectPassed_ReturnsCorrectMembership()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _coursesController.GetCourseMembership(testCourseId, testUserId);

            var okObjectResult = response.Result as OkObjectResult;

            var item = Assert.IsType<MembershipDto>(okObjectResult.Value);
            Assert.Equal(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), item.CourseId);
        }

        [Fact]
        public void DeleteCourseMembership_UnknownUserPassed_ReturnsNotFoundObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.NewGuid();

            var response = _coursesController.DeleteCourseMembership(testCourseId, testUserId);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }


        [Fact]
        public void DeleteCourseMembership_UnknownCoursePassed_ReturnsNotFoundObjectResult()
        {
            var testCourseId = Guid.NewGuid();
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _coursesController.DeleteCourseMembership(testCourseId, testUserId);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public void DeleteCourseMembership_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _coursesController.DeleteCourseMembership(testCourseId, testUserId);

            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public void DeleteCourseMembership_ValidObjectPassed_ReturnsCorrectMembership()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var response = _coursesController.DeleteCourseMembership(testCourseId, testUserId);

            var okObjectResult = response.Result as OkObjectResult;

            var item = Assert.IsType<MembershipDto>(okObjectResult.Value);
            Assert.Equal(Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"), item.UserId);
        }
    }
}
