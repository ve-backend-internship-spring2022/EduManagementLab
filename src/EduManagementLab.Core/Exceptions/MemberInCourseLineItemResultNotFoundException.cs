using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class MemberInCourseLineItemResultNotFoundException : Exception
    {
        public MemberInCourseLineItemResultNotFoundException(Guid memberId) : base($"Course line item result for member {memberId} not found")
        {
        }
    }
}
