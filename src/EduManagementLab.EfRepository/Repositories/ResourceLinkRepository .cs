using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.EfRepository.Repositories
{
    internal class ResourceLinkRepository : GenericRepository<IMSLTIResourceLink>, IResourceLinkRepository
    {
        public readonly DataContext _context;
        public ResourceLinkRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<IMSLTIResourceLink> GetResourceLinks(bool includeTool = false)
        {
            if (includeTool == false)
            {
                _context.IMSLTIResourceLinks.ToList();
            }
            return _context.IMSLTIResourceLinks.Include(t => t.Tool).ToList();
        }
        public IMSLTIResourceLink GetResourceLink(Guid id, bool includeTool = false)
        {
            if (includeTool == false)
            {
                return _context.IMSLTIResourceLinks.FirstOrDefault(r => r.Id == id);
            }
            return _context.IMSLTIResourceLinks.Include(t => t.Tool).FirstOrDefault(r => r.Id == id);
        }
    }
}
