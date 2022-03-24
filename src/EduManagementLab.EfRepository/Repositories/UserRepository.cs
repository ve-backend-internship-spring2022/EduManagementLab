using EduManagementLab.Core.Interfaces.Repositories;
using EduManagementLab.Core.Entities;

namespace EduManagementLab.EfRepository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }

        public Task<User> FindBySubjectId(string subjectId)
        {
            throw new NotImplementedException();
        }

        public User FindByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public bool ValidateCredentials(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
