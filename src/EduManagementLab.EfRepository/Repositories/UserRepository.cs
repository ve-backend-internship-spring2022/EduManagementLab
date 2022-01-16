using EduManagementLab.Core.Interfaces.Repositories;
using EduManagementLab.Core.Entities;

namespace EduManagementLab.EfRepository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }
    }
}
