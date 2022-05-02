using EduManagementLab.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface IResourceLinkRepository: IGenericRepository<IMSLTIResourceLink>
    {
        IEnumerable<IMSLTIResourceLink>? GetResourceLinks(bool includeTool);
        IMSLTIResourceLink? GetResourceLink(Guid id, bool includeTool);
    }
}
