using EduManagementLab.Core.Entities;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Course? GetCourse(Guid courseId, bool includeMembershipUsers);
    }
}
