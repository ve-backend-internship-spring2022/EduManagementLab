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
            return _context.CourseMemberships.Include(c => c.Course).Where(p => p.UserId == userId).ToList();
        }
        public Course? GetCourse(Guid courseId, bool includeMembershipUsers)
        {
            if (includeMembershipUsers == true)
            {
                return _context.Courses.Include(c => c.CourseTasks).ThenInclude(r => r.Results).Include(m => m.Memperships).ThenInclude(u => u.User).FirstOrDefault(m => m.Id == courseId);
            }
            else
            {
                return _context.Courses.Include(m => m.CourseTasks).FirstOrDefault(m => m.Id == courseId);
            }
        }
    }
}
