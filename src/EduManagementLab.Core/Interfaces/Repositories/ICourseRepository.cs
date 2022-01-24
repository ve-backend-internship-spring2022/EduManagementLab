using EduManagementLab.Core.Entities;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Course? GetCourseIncludeMemberships(Guid courseId, bool membership, bool user);
    }
}
