using Xunit;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.EfRepository.Tests
{
    public class Database
    {
        [Fact]
        public void DropCreateAndSeed_EduManagementLabDb()
        {
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EduManagementLabDb;Trusted_Connection=True;MultipleActiveResultSets=true")
                    .Options;

            var dataContext = new DataContext(dbContextOptions);

            dataContext.Database.EnsureDeleted();
            dataContext.Database.EnsureCreated();

            DevTestData.Seed(dataContext);

            Assert.NotEmpty(new UnitOfWork(dataContext).Users.GetAll());
        }
    }
}