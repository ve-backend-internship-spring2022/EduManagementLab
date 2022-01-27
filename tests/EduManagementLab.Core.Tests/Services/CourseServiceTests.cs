using System;
using Xunit;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using EduManagementLab.Core.Entities;
using System.Linq;

namespace EduManagementLab.Core.Tests.Services
{
    public class CourseServiceTests
    {
        private readonly DbContextOptions<DataContext> _contextOptions;


        public CourseServiceTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("CourseServiceTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new DataContext(_contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.AddRange(
                new Course { Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), Code = "AAA", Name = "CourseNameOne", Description = "CourseDescriptionOne", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue },
                new Course { Id = Guid.Parse("4A0E4335-08E0-45C0-8A97-9791CE81E73D"), Code = "BBB", Name = "CourseNameTwo", Description = "CourseDescriptionTwo", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue });

            context.SaveChanges();
        }

        [Fact]
        public void GetCourse()
        {
            using var context = CreateContext();

            var unitOfWorkGet = new UnitOfWork(context);
            var courseServiceGet = new CourseService(unitOfWorkGet);
            var fetchedCourse = courseServiceGet.GetCourse(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"));

            Assert.NotNull(fetchedCourse);
            Assert.Equal("AAA", fetchedCourse.Code);
            Assert.Equal("CourseNameOne", fetchedCourse.Name);
            Assert.Equal("CourseDescriptionOne", fetchedCourse.Description);
            Assert.Equal(DateTime.MinValue, fetchedCourse.StartDate);
            Assert.Equal(DateTime.MaxValue, fetchedCourse.EndDate);
        }

        [Fact]
        public void GetAllCourses()
        {
            using var context = CreateContext();

            var unitOfWorkGet = new UnitOfWork(context);
            var courseServiceGet = new CourseService(unitOfWorkGet);
            var fetchedCourses = courseServiceGet.GetCourses();

            Assert.Collection(
                fetchedCourses,
                b => Assert.Equal("AAA", b.Code),
                b => Assert.Equal("BBB", b.Code));
        }

        [Fact]
        public void AddCourse()
        {
            using var context = CreateContext();

            var unitOfWorkCreate = new UnitOfWork(context);
            var courseServiceCreate = new CourseService(unitOfWorkCreate);
            var createdCourse = courseServiceCreate.CreateCourse("CCC", "CourseNameThree", "CourseDescriptionThree", DateTime.Today, DateTime.Today);

            var course = context.Courses.Single(b => b.Code == "CCC");
            Assert.Equal("CCC", course.Code);
        }

        [Fact]
        public void UpdateCourse()
        {
            using var context = CreateContext();


            var unitOfWorkUpdate = new UnitOfWork(context);
            var courseServiceUpdate = new CourseService(unitOfWorkUpdate);

            courseServiceUpdate.UpdateCourseInfo(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), "AAB", "ChangedCourseName", "ChangedCourseDescription");
            courseServiceUpdate.UpdateCoursePeriod(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), DateTime.Today, DateTime.Today);


            var course = context.Courses.Single(b => b.Code == "AAB");

            Assert.NotNull(course);
            Assert.Equal("AAB", course.Code);
            Assert.Equal("ChangedCourseName", course.Name);
            Assert.Equal("ChangedCourseDescription", course.Description);
            Assert.Equal(DateTime.Today, course.StartDate);
            Assert.Equal(DateTime.Today, course.EndDate);
        }

        DataContext CreateContext() => new DataContext(_contextOptions);

    }
}
