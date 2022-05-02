using EduManagementLab.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.EfRepository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Course.Membership> CourseMemberships { get; set; }
        public DbSet<CourseTask> CourseTasks { get; set; }
        public DbSet<CourseTask.Result> LineItemResults { get; set; }
    }
}
