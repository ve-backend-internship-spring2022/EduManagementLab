using EduManagementLab.Core.Interfaces.Repositories;
using EduManagementLab.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.EfRepository.Repositories
{
    internal class CourseTaskRepository : GenericRepository<CourseTask>, ICourseTaskRepository
    {
        public readonly DataContext _context;
        public CourseTaskRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public CourseTask? GetCourseTask(Guid courseTaskId, bool includeResults)
        {
            if (includeResults == true)
            {
                return _context.CourseTasks.Include(c => c.Results).ThenInclude(c => c.Membership).FirstOrDefault(c => c.Id == courseTaskId);
            }
            else
            {
                return _context.CourseTasks.FirstOrDefault(c => c.Id == courseTaskId);
            }
        }
    }
}
