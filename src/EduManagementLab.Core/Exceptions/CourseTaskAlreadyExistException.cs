using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class CourseTaskAlreadyExistException : Exception
    {
        public CourseTaskAlreadyExistException(string name) : base($"Course line item: {name} already exist")
        {
        }
    }
}
