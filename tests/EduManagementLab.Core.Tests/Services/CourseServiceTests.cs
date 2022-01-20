using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.Core.Tests.Services
{
    public class CourseServiceTests
    {
        [Fact]
        public void CreateCourse()
        {

            string databaseName = "EduManagementLabDb_Test_CreteUpdateAndGetUser";
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            var dataContext = new DataContext(dbContextOptions);

            dataContext.Database.EnsureDeleted();
            dataContext.Database.EnsureCreated();

            var unitOfWorkCreate = new UnitOfWork(dataContext);
            var courseServiceCreate = new CourseService(unitOfWorkCreate);
            var createdCourse = courseServiceCreate.CreateCourse("AAA", "CourseName", "CourseDescription", DateTime.Today, DateTime.Today);

            var unitOfWorkGet = new UnitOfWork(dataContext);
            var courseServiceGet = new CourseService(unitOfWorkGet);
            var fetchedCourse = courseServiceGet.GetCourse(createdCourse.Id);

            Assert.NotNull(fetchedCourse);
            Assert.Equal("AAA", createdCourse.Code);
            Assert.Equal("CourseName", createdCourse.Name);
            Assert.Equal("CourseDescription", createdCourse.Description);
            Assert.Equal(DateTime.Today, createdCourse.StartDate);
            Assert.Equal(DateTime.Today, createdCourse.EndDate);

            dataContext.Database.EnsureDeleted();
        }

        [Fact]
        public void UpdateCourse()
        {

            string databaseName = "EduManagementLabDb_Test_CreteUpdateAndGetUser";
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(@$"Server=(localdb)\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            var dataContext = new DataContext(dbContextOptions);

            dataContext.Database.EnsureDeleted();
            dataContext.Database.EnsureCreated();

            var unitOfWorkCreate = new UnitOfWork(dataContext);
            var courseServiceCreate = new CourseService(unitOfWorkCreate);
            var createdCourse = courseServiceCreate.CreateCourse("AAA", "CourseName", "CourseDescription", DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2));

            var unitOfWorkUpdate = new UnitOfWork(dataContext);
            var courseServiceUpdate = new CourseService(unitOfWorkUpdate);


            courseServiceUpdate.UpdateCourseInfo(createdCourse.Id, "BBB", "ChangedCourseName", "ChangedCourseDescription");
            courseServiceUpdate.UpdateCoursePeriod(createdCourse.Id, DateTime.Today, DateTime.Today);

            var unitOfWorkGet = new UnitOfWork(dataContext);
            var courseServiceGet = new CourseService(unitOfWorkGet);
            var fetchedCourse = courseServiceGet.GetCourse(createdCourse.Id);

            Assert.NotNull(fetchedCourse);
            Assert.Equal("BBB", createdCourse.Code);
            Assert.Equal("ChangedCourseName", createdCourse.Name);
            Assert.Equal("ChangedCourseDescription", createdCourse.Description);
            Assert.Equal(DateTime.Today, createdCourse.StartDate);
            Assert.Equal(DateTime.Today, createdCourse.EndDate);

            dataContext.Database.EnsureDeleted();
        }


    }

}
