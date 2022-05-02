using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EduManagementLab.Core.Tests.Services
{
    public class CourseTasksServiceTests
    {
        private readonly DbContextOptions<DataContext> _contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("CourseTasksServiceTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        private readonly DataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly CourseService _courseService;
        private readonly CourseTaskService _courseTaskService;
        private readonly UserService _userService;
        DataContext CreateContext() => new DataContext(_contextOptions);
        UnitOfWork CreateUnitOfWork() => new UnitOfWork(_dataContext);
        CourseTaskService CreateCourseTask() => new CourseTaskService(_unitOfWork);
        CourseService CreateCourseService() => new CourseService(_unitOfWork);
        UserService CreateUserService() => new UserService(_unitOfWork);
        public CourseTasksServiceTests()
        {

            _dataContext = CreateContext();

            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _unitOfWork = CreateUnitOfWork();
            _userService = CreateUserService();
            _courseService = CreateCourseService();
            _courseTaskService = CreateCourseTask();

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
        }

        [Fact]
        public void GetCourseTask_ExistingObjectIdPassed_ReturnsCorrectCourseTask()
        {
            var fetchedCourseTask = _courseTaskService.GetCourseTask(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));

            Assert.NotNull(fetchedCourseTask);
            Assert.Equal("Assignment1", fetchedCourseTask.Name);
            Assert.Equal("", fetchedCourseTask.Description);
            Assert.Equal(DateTime.MinValue, fetchedCourseTask.LastUpdate);
        }

        [Fact]
        public void GetCourseTask_UnknownObjectIdPassed_ThrowsCourseTaskNotFoundException()
        {
            Assert.Throws<CourseTaskNotFoundException>(() => _courseTaskService.GetCourseTask(Guid.NewGuid()));
        }

        [Fact]
        public void GetCourseTasks_ReturnsCorrectCourseTasks()
        {
            var fetchedCourseTask = _courseTaskService.GetCourseTasks();

            Assert.Collection(
                fetchedCourseTask,
                c => Assert.Equal("Assignment1", c.Name),
                c => Assert.Equal("Assignment2", c.Name));
        }

        [Fact]
        public void CreateCourseTask_ReturnsCorrectCourseTask()
        {
            var createdCourse = _courseService.GetCourse(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), true);
            var createdCourseLineItem = _courseTaskService.CreateCourseTask(createdCourse.Id, "Assignment3", "DescAssignment3");

            var courseTask = _dataContext.CourseTasks.Single(b => b.Name == "Assignment3");
            Assert.Equal("Assignment3", courseTask.Name);
        }

        [Fact]
        public void CreateCourseTask_ExistingObjectNamePassed_ThrowsCourseTaskAlreadyExistException()
        {
            Assert.Throws<CourseTaskAlreadyExistException>(() => _courseTaskService.CreateCourseTask(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), "Assignment1", ""));
        }

        [Fact]
        public void UpdateCourseTaskInfo_ReturnsCorrectCourseTask()
        {
            _courseTaskService.UpdateCourseTask(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"), "Assignment1B", "ChangedDescription");

            var courseTask = _dataContext.CourseTasks.FirstOrDefault(b => b.Id == Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));

            Assert.NotNull(courseTask);
            Assert.Equal("Assignment1B", courseTask.Name);
            Assert.Equal("ChangedDescription", courseTask.Description);
        }

        [Fact]
        public void DeleteCourseTask_DeletesCorrectCourseTask()
        {
            _courseTaskService.DeleteCourseTask(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));

            var fetchedCourseTasks = _courseTaskService.GetCourseTasks();

            Assert.Single(fetchedCourseTasks);
        }

        [Fact]
        public void CreateCourseTaskResult_ReturnsCorrectCourseTaskResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.NewGuid();
            var testScore = 50.00M;

            var courseTaskResult = _courseTaskService.CreateCourseTaskResult(testCourseTaskId, testMemberId, testScore);

            Assert.Equal(testCourseTaskId, courseTaskResult.CourseTaskId);
        }

        [Fact]
        public void CreateCourseTaskResult_UnknownCourseTaskIdPassed_ThrowsCourseTaskNotFoundException()
        {
            var testCourseTaskId = Guid.NewGuid();
            var testMemberId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");
            var testScore = 50.00M;

            Assert.Throws<CourseTaskNotFoundException>(() => _courseTaskService.CreateCourseTaskResult(testCourseTaskId, testMemberId, testScore));
        }

        [Fact]
        public void CreateCourseTaskResult_ExistingObjectPassed_ThrowsMembershipAlreadyExistException()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testScore = 50.00M;

            Assert.Throws<MemberInCourseLineItemResultAlreadyExistException>(() => _courseTaskService.CreateCourseTaskResult(testCourseTaskId, testMemberId, testScore));
        }

        [Fact]
        public void DeleteCourseTask_ReturnsCorrectCourseTask()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var courseTaskResult = _courseTaskService.DeleteCourseTask(testCourseTaskId);

            Assert.Equal(testCourseTaskId, courseTaskResult.Id);
        }

        [Fact]
        public void DeleteCourseTask_UnknownCourseTaskId_ThrowsCourseTaskNotFoundException()
        {
            var testCourseTaskId = Guid.NewGuid();

            Assert.Throws<CourseTaskNotFoundException>(() => _courseTaskService.DeleteCourseTask(testCourseTaskId));
        }

        [Fact]
        public void UpdateCourseTaskResult_ReturnsCorrectCourseTaskResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testScore = 80.00M;

            var courseTaskResult = _courseTaskService.UpdateCourseTaskResult(testCourseTaskId, testMemberId, testScore);

            Assert.Equal(testScore, courseTaskResult.Score);
        }

        [Fact]
        public void UpdateCourseTaskResult_UnknownCourseTaskId_ThrowsCourseTaskNotFoundException()
        {
            var testCourseTaskId = Guid.NewGuid();
            var testMemberId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");
            var testScore = 60.00M;

            Assert.Throws<CourseTaskNotFoundException>(() => _courseTaskService.UpdateCourseTaskResult(testCourseTaskId, testMemberId, testScore));
        }

        [Fact]
        public void UpdateCourseTaskResult_UnknownMemberId_ThrowsCourseTaskNotFoundException()
        {
            var testCourseTaskId = Guid.Parse("116E1A55-C816-425E-B385-FB476586DB7F");
            var testMemberId = Guid.NewGuid();
            var testScore = 60.00M;

            Assert.Throws<CourseTaskNotFoundException>(() => _courseTaskService.UpdateCourseTaskResult(testCourseTaskId, testMemberId, testScore));
        }

        [Fact]
        public void DeleteCourseTaskResult_ReturnsCorrectCourseTaskResult()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");

            var courseTaskResult = _courseTaskService.DeleteCourseTaskResult(testCourseTaskId, testMemberId);

            Assert.Equal(testCourseTaskId, courseTaskResult.CourseTaskId);
        }

        [Fact]
        public void DeleteCourseTaskResult_UnknownMemberId_ThrowsCourseTaskResultNotFoundException()
        {
            var testCourseTaskId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.NewGuid();

            Assert.Throws<MemberInCourseTaskResultNotFoundException>(() => _courseTaskService.DeleteCourseTaskResult(testCourseTaskId, testMemberId));
        }

        [Fact]
        public void DeleteCourseTaskResult_UnknownCourseTaskId_ThrowsCourseTaskResultNotFoundException()
        {
            var testCourseTaskId = Guid.NewGuid();
            var testMemberId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            Assert.Throws<CourseTaskNotFoundException>(() => _courseTaskService.DeleteCourseTaskResult(testCourseTaskId, testMemberId));
        }
    }
}
