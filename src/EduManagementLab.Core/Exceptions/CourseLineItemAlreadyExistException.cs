using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class CourseLineItemAlreadyExistException : Exception
    {
        public CourseLineItemAlreadyExistException(string name) : base($"Course line item: {name} already exist")
        {
        }
    }
}
