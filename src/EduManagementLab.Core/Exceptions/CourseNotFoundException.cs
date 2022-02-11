using System;

namespace EduManagementLab.Core.Exceptions
{
    public class CourseNotFoundException : Exception
    {
        public CourseNotFoundException(Guid courseId) : base($"No course found with id {courseId}")
        {
        }
    }
}
