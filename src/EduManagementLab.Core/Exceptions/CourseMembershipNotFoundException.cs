using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class CourseMembershipNotFoundException : Exception
    {
        public CourseMembershipNotFoundException(Guid courseId, Guid userId) : base($"User:{userId} not found in Course:{courseId}")
        {
        }
    }
}
