using EduManagementLab.Core.Interfaces;
using EduManagementLab.Core.Interfaces.Repositories;
using EduManagementLab.EfRepository.Repositories;

namespace EduManagementLab.EfRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        public UnitOfWork(DataContext context)
        {
            _context = context;

            Users = new UserRepository(_context);
            Courses = new CourseRepository(_context);
            CourseTasks = new CourseTaskRepository(_context);
            CourseTaskResults = new CourseTaskResultsRepository(_context);
            Tools = new ToolRepository(_context);
            ResourceLinks = new ResourceLinkRepository(_context);

        }

        public IUserRepository Users { get; private set; }
        public ICourseRepository Courses { get; private set; }
        public ICourseTaskRepository CourseTasks { get; set; }
        public ICourseTaskResultsRepository CourseTaskResults { get; }
        public IToolRepository Tools { get; }
        public IResourceLinkRepository ResourceLinks { get; }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
