using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class CourseAlreadyExistException : Exception
    {
        public CourseAlreadyExistException() : base($"Course code/name already exist")
        {
        }
    }
}
