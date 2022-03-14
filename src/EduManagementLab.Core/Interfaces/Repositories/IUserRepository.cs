using EduManagementLab.Core.Entities;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        bool ValidateCredentials(string username, string password);

        Task<User> FindBySubjectId(string subjectId);

        User FindByUsername(string username);
    }
}
