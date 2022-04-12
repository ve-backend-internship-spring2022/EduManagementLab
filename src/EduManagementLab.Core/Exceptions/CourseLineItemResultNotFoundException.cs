using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class CourseLineItemResultNotFoundException : Exception
    {
        public CourseLineItemResultNotFoundException(Guid lineItemResultId) : base($"CourseLineItemResult:{lineItemResultId} not found")
        {
        }
    }
}
