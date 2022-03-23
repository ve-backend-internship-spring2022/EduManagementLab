using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Interfaces.Repositories;
using EduManagementLab.EfRepository.Repositories;

namespace EduManagementLab.EfRepository
{
    internal class LineItemResultsRepository : GenericRepository<CourseLineItem.Result>, ILineItemResultsRepository
    {
        public readonly DataContext _context;
        public LineItemResultsRepository(DataContext context) : base(context)
        {
            _context = context;
        }


    }
}