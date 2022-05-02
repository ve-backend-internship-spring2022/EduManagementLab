using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduManagementLab.Core.Entities;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface ICourseLineItemRepository : IGenericRepository<CourseLineItem>
    {
        CourseLineItem? GetCourseLineItem(Guid lineItemId, bool includeResults, bool includeResource);
    }
}
