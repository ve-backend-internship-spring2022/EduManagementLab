using EduManagementLab.Core.Interfaces.Repositories;


namespace EduManagementLab.Core.Interfaces
{
    public interface IUnitOfWork
    {

        IUserRepository Users { get; }
        ICourseRepository Courses { get; }
        ICourseTaskRepository CourseTasks { get; }
        ICourseTaskResultsRepository CourseTaskResults { get; }
        int Complete();
    }
}
