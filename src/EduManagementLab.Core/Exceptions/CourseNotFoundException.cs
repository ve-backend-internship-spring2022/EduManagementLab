using System;

namespace EduManagementLab.Core.Exceptions
{
    public class CourseNotFoundException : Exception
    {
        public CourseNotFoundException(Guid userId) : base($"No course found with id {userId}")
        {
        }
    }
}
