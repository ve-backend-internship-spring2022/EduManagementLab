using EduManagementLab.Core.Interfaces.Repositories;
using EduManagementLab.Core.Entities;

namespace EduManagementLab.EfRepository.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(DataContext context) : base(context)
        {
        }
    }
}
