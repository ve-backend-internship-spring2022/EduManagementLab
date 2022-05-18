using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.EfRepository.Repositories
{
    internal class ToolRepository : GenericRepository<Tool>, IToolRepository
    {
        public readonly DataContext _context;
        public ToolRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
