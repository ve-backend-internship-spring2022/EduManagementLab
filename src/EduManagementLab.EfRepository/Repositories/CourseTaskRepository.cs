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
        public List<CourseTask>? GetCourseTasks( bool includeResults, bool includeResource)
        {
            if (includeResults == true || includeResource == true)
            {
                return _context.CourseTasks.Include(r => r.IMSLTIResourceLinks).ThenInclude(t => t.Tool).Include(c => c.Results).ThenInclude(c => c.Membership).ToList();
            }
            else
            {
                return _context.CourseTasks.ToList();
            }
        }
        public CourseTask? GetCourseTask(Guid courseTaskId, bool includeResults, bool includeResource)
        {
            if (includeResults == true || includeResource == true)
            {
                return _context.CourseTasks.Include(r => r.IMSLTIResourceLinks).ThenInclude(t => t.Tool).Include(c => c.Results).ThenInclude(c => c.Membership).FirstOrDefault(c => c.Id == courseTaskId);
            }
            else
            {
                return _context.CourseTasks.FirstOrDefault(c => c.Id == courseTaskId);
            }
        }
    }
}
