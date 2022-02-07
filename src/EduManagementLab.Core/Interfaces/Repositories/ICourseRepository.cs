using EduManagementLab.Core.Entities;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        IEnumerable<Course.Membership> GetUserCourses(Guid userId);
        Course? GetCourse(Guid courseId, bool includeMembershipUsers);
    }
}
