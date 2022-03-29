using EduManagementLab.Core.Interfaces.Repositories;
using EduManagementLab.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.EfRepository.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public readonly DataContext _context;
        public CourseRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Course.Membership> GetUserCourses(Guid userId)
        {
            return _context.Courses
                .Include(m => m.Memperships)
                .ThenInclude(u => u.User)
                .Where(m => m.Memperships.Any(p => p.UserId == userId))
                .Select(c => new Course.Membership()
                {
                    Course = new Course()
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Description = c.Description,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                    },
                });
        }
        public Course? GetCourse(Guid courseId, bool includeMembershipUsers)
        {
            if (includeMembershipUsers == true)
            {
                return _context.Courses.Include(c => c.CourseLineItems).ThenInclude(r => r.Results).Include(m => m.Memperships).ThenInclude(u => u.User).FirstOrDefault(m => m.Id == courseId);
            }
            else
            {
                //return _context.Courses.FirstOrDefault(c => c.Id == courseId);
                return _context.Courses.Include(m => m.CourseLineItems).FirstOrDefault(m => m.Id == courseId);
            }
        }
    }
}
