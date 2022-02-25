using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Interfaces;

namespace EduManagementLab.Core.Services
{
    public static class Guard
    {
        public static void AgainstUnknownCourseMembership(Course course, Guid userId)
        {
            if (!course.Memperships.Any(c => c.UserId == userId))
            {
                throw new CourseMembershipNotFoundException(course.Id, userId);
            }
        }
        public static void AgainstNullUser(Guid userId, IUnitOfWork unitOfWork)
        {
            var user = unitOfWork.Users.GetById(userId);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
        }
        public static void AgainstNullCourse(Guid courseId, IUnitOfWork unitOfWork)
        {
            var course = unitOfWork.Courses.GetById(courseId);

            if (course == null)
            {
                throw new CourseNotFoundException(courseId);
            }
        }
        public static void AgainstDuplicateMembership(Guid courseId, Guid userId, IUnitOfWork unitOfWork)
        {
            var course = unitOfWork.Courses.GetCourse(courseId, true);

            if (course.Memperships.Any(c => c.UserId == userId))
            {
                throw new MembershipAlreadyExistException();
            }
        }
    }

}
