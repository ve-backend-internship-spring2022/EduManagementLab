using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Interfaces.Repositories;
using EduManagementLab.EfRepository.Repositories;

namespace EduManagementLab.EfRepository
{
    internal class CourseTaskResultsRepository : GenericRepository<CourseTask.Result>, ICourseTaskResultsRepository
    {
        public readonly DataContext _context;
        public CourseTaskResultsRepository(DataContext context) : base(context)
        {
            _context = context;
        }


    }
}