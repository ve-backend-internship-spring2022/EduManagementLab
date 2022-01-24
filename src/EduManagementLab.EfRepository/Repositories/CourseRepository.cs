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
        public Course? GetCourseIncludeMemberships(Guid courseId, bool membership, bool user)
        {
            if (membership == true && user == false)
            {
                return _context.Courses.Include(m => m.Memperships).FirstOrDefault(c => c.Id == courseId);
            }
            else if (membership == true && user == true)
            {
                return _context.Courses.Include(m => m.Memperships).ThenInclude(u => u.User).FirstOrDefault(m => m.Id == courseId);
            }
            else
            {
                return _context.Courses.FirstOrDefault(c => c.Id == courseId);
            }
        }
    }
}
