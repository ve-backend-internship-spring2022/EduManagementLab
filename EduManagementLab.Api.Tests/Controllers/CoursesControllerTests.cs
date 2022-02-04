﻿using AutoMapper;
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
        public void GetCourse_ExistingGuidPassed_ReturnsOkObjectResult()
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

            var createdResponse = _coursesController.AddCourse(testCreateCourseRequest);

            Assert.IsType<CreatedAtActionResult>(createdResponse.Result);
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
        public void UpdateCourseInfo_ExistingObjectIdPassed_ReturnsOkObjectResult()
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
        public void UpdateCoursePeriod_ExistingObjectIdPassed_ReturnsOkObjectResult()
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

        [Fact]
        public void DeleteCourse_UnknownObjectIdPassed_ReturnsNotFoundResult()
        {
            var testCourseId = Guid.NewGuid();

            var deleteResponse = _coursesController.DeleteCourse(testCourseId);

            Assert.IsType<NotFoundResult>(deleteResponse);
        }

        [Fact]
        public void DeleteCourse_ExistingObjectIdPassed_ReturnsOkResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");

            var deleteResponse = _coursesController.DeleteCourse(testCourseId);

            Assert.IsType<OkResult>(deleteResponse);
        }

        [Fact]
        public void AddCourseMembership_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.NewGuid();
            var testEnrollmentDate = DateTime.MinValue;

            var addMembershipResponse = _coursesController.AddCourseMembership(testCourseId, testUserId, testEnrollmentDate);

            Assert.IsType<OkObjectResult>(addMembershipResponse.Result);
        }

        [Fact]
        public void AddCourseMembership_UnknownObjectPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.NewGuid();
            var testUserId = Guid.NewGuid();
            var testEnrollmentDate = DateTime.MinValue;

            var addMembershipResponse = _coursesController.AddCourseMembership(testCourseId, testUserId, testEnrollmentDate);

            Assert.IsType<BadRequestObjectResult>(addMembershipResponse.Result);
        }

        [Fact]
        public void AddCourseMembership_ValidObjectPassed_ReturnsUpdatedObject()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.NewGuid();
            var testEnrollmentDate = DateTime.MinValue;

            var addMembershipResponse = _coursesController.AddCourseMembership(testCourseId, testUserId, testEnrollmentDate);

            var updateResult = addMembershipResponse.Result as OkObjectResult;

            var item = updateResult.Value as CourseDto;

            Assert.IsType<CourseDto>(item);
            Assert.Equal(item.Id, testCourseId);
        }

        [Fact]
        public void UpdateCourseMembershipEnrolledDate_UnknownObjectPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.NewGuid();
            var testUserId = Guid.NewGuid();
            var testEnrollmentDate = DateTime.MinValue;

            var updateMembershipResponse = _coursesController.UpdateCourseMembershipEnrolledDate(testCourseId, testUserId, testEnrollmentDate);

            Assert.IsType<BadRequestObjectResult>(updateMembershipResponse.Result);
        }

        [Fact]
        public void UpdateCourseMembershipEnrolledDate_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.NewGuid();
            var testEnrollmentDate = DateTime.MinValue;

            var updateMembershipResponse = _coursesController.UpdateCourseMembershipEnrolledDate(testCourseId, testUserId, testEnrollmentDate);

            Assert.IsType<OkObjectResult>(updateMembershipResponse.Result);
        }

        [Fact]
        public void UpdateCourseMembershipEnrolledDate_ValidObjectPassed_ReturnsUpdatedObject()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.NewGuid();
            var testEnrollmentDate = DateTime.MinValue;

            var updateMembershipResponse = _coursesController.UpdateCourseMembershipEnrolledDate(testCourseId, testUserId, testEnrollmentDate);

            var updateResult = updateMembershipResponse.Result as OkObjectResult;

            var item = updateResult.Value as CourseDto;

            Assert.IsType<CourseDto>(item);
            Assert.Equal(item.Id, testCourseId);
        }

        [Fact]
        public void GetCourseMemberships_UnknownObjectPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.NewGuid();

            var getMembershipsResponse = _coursesController.GetCourseMemberships(testCourseId);

            Assert.IsType<NotFoundResult>(getMembershipsResponse.Result);
        }

        [Fact]
        public void GetCourseMemberships_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");

            var getMembershipsResponse = _coursesController.GetCourseMemberships(testCourseId);

            Assert.IsType<OkObjectResult>(getMembershipsResponse.Result);
        }

        [Fact]
        public void GetCourseMemberships_ValidObjectPassed_ReturnsCorrectMemberships()
        {

            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");

            var getMembershipsResponse = _coursesController.GetCourseMemberships(testCourseId);

            var okResult = getMembershipsResponse.Result as OkObjectResult;

            var items = Assert.IsType<List<MembershipDto>>(okResult.Value);
            Assert.Equal(1, items.Count);
            Assert.Equal(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), items[0].CourseId);
        }

        [Fact]
        public void GetCourseMembership_UnknownCourseIdPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.NewGuid();
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var getMembershipResponse = _coursesController.GetCourseMembership(testCourseId, testUserId);

            Assert.IsType<NotFoundResult>(getMembershipResponse.Result);
        }

        [Fact]
        public void GetCourseMembership_UnknownUserIdPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.NewGuid();

            var getMembershipResponse = _coursesController.GetCourseMembership(testCourseId, testUserId);

            Assert.IsType<NotFoundResult>(getMembershipResponse.Result);
        }

        [Fact]
        public void GetCourseMembership_ValidObjectPassed_ReturnsOkObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var getMembershipResponse = _coursesController.GetCourseMembership(testCourseId, testUserId);

            Assert.IsType<OkObjectResult>(getMembershipResponse.Result);
        }

        [Fact]
        public void GetCourseMembership_ValidObjectPassed_ReturnsCorrectMembership()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var getMembershipResponse = _coursesController.GetCourseMembership(testCourseId, testUserId);

            var okResult = getMembershipResponse.Result as OkObjectResult;

            var item = Assert.IsType<MembershipDto>(okResult.Value);
            Assert.Equal(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), item.CourseId);
        }

        [Fact]
        public void DeleteCourseMembership_UnknownObjectPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.NewGuid();
            var testUserId = Guid.NewGuid();

            var getMembershipResponse = _coursesController.DeleteCourseMembership(testCourseId, testUserId);

            Assert.IsType<NotFoundObjectResult>(getMembershipResponse.Result);
        }

        [Fact]
        public void DeleteCourseMembership_ValidObjectPassed_ReturnsBadRequestObjectResult()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var getMembershipResponse = _coursesController.DeleteCourseMembership(testCourseId, testUserId);

            Assert.IsType<NotFoundObjectResult>(getMembershipResponse.Result);
        }

        [Fact]
        public void DeleteCourseMembership_ValidObjectPassed_ReturnsCorrectMembership()
        {
            var testCourseId = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB");
            var testUserId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            var deleteMembershipResponse = _coursesController.DeleteCourseMembership(testCourseId, testUserId);

            var okObjectResult = deleteMembershipResponse.Result as OkObjectResult;

            var item = Assert.IsType<CourseDto>(okObjectResult.Value);
            Assert.Equal(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), item.Id);
        }
    }
}
