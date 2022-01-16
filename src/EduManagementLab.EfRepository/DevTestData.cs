using EduManagementLab.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.EfRepository
{
    public static class DevTestData
    {
        public static void Seed(DataContext dataContext)
        {
            var unitOfWork = new UnitOfWork(dataContext);
            unitOfWork.Users.AddRange(GetDevTestUsers().ToList());
            unitOfWork.Courses.AddRange(GetDevTestCourses().ToList());
            unitOfWork.Complete();
        }

        public static IEnumerable<User> GetDevTestUsers()
        {
            return new List<User>
            {
                new User() { Id = Guid.Parse("{106CD9D2-3065-47A8-AC34-85343037534A}"), Displayname = "Mr John", FirstName = "John", LastName = "Doe", Email = "john.doe@myemail.test" },
                new User() { Id = Guid.Parse("{BF6DA545-C0A1-49FB-86AC-4BD6B890C3F5}"), Displayname = "Anders S", FirstName = "Anders", LastName = "Svensson", Email = "anders.svensson@myemail.test" }
            };
        }

        public static IEnumerable<Course> GetDevTestCourses()
        {
            return new List<Course>
            {
                new Course() { Id = Guid.Parse("{56E96EA1-90D5-47E9-A1FA-93073614B7FB}"), Code = "ECO1", Name = "Economy 1", Description = "Introduction course to economy", StartDate = DateTime.Now.AddDays(-60), EndDate = DateTime.Now.AddDays(-30)},
                new Course() { Id = Guid.Parse("{1BFE9361-8CEE-42A8-881D-28BAC263B6B9}"), Code = "ECO2", Name = "Economy 2", StartDate = DateTime.Now.AddDays(-29), EndDate = DateTime.Now.AddDays(40) },
                new Course() { Id = Guid.Parse("{A94CC958-3777-4814-BCB7-5DB3F7A25D74}"), Code = "MAN1", Name = "Management 1", Description = "Introduction course to management", StartDate = DateTime.Now.AddDays(-25), EndDate = DateTime.Now.AddDays(30) },
                new Course() { Id = Guid.Parse("{18B9CDAE-BED0-468A-9491-E2394212CA75}"), Code = "MAN2", Name = "Management 2", StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(45) }
            };
        }
    }
}
