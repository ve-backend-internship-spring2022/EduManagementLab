using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class CourseTaskNotFoundException : Exception
    {
        public CourseTaskNotFoundException(Guid courseLineItemId) : base($"No Course Line Item found with id {courseLineItemId}")
        {
        }
    }
}
