using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduManagementLab.Core.Entities;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface ICourseTaskRepository : IGenericRepository<CourseTask>
    {
        List<CourseTask>? GetCourseTasks(bool includeResults, bool includeResource);
        CourseTask? GetCourseTask(Guid courseTaskId, bool includeResults, bool includeResource);
    }
}
