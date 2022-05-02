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

        public static void AgainstDuplicateMembership(Guid courseId, Guid userId, IUnitOfWork unitOfWork)
        {
            var course = unitOfWork.Courses.GetCourse(courseId, true);

            if (course.Memperships.Any(c => c.UserId == userId))
            {
                throw new MembershipAlreadyExistException(userId);
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

        public static void AgainstDuplicateCourseTaskResult(Guid lineItemId, Guid memberId, IUnitOfWork unitOfWork)
        {
            var courseLineItem = unitOfWork.CourseTasks.GetCourseTask(lineItemId, true);

            if(courseLineItem.Results.Any(c => c.MembershipId == memberId && c.CourseTaskId == lineItemId))
            {
                throw new MemberInCourseLineItemResultAlreadyExistException(memberId);
            }
        }

        public static void AgainstUnknownMemberInCourseTaskResult(Guid lineItemId, Guid memberId, IUnitOfWork unitOfWork)
        {
            var courseLineItem = unitOfWork.CourseTasks.GetCourseTask(lineItemId, true);

            if (!courseLineItem.Results.Any(c => c.MembershipId == memberId && c.CourseTaskId == lineItemId))
            {
                throw new MemberInCourseTaskResultNotFoundException(memberId);
            }
        }

        public static void AgainstUnknownCourseTask(Guid lineItemId, IUnitOfWork unitOfWork)
        {
            var courseLineItem = unitOfWork.CourseTasks.GetCourseTask(lineItemId, true);

            if (!courseLineItem.Results.Any(x => x.CourseTaskId == lineItemId))
            {
                throw new CourseTaskNotFoundException(lineItemId);
            }
        }

        public static void AgaintDuplicateNameInCourseTask(Guid courseId, string name, IUnitOfWork unitOfWork)
        {
            var course = unitOfWork.Courses.GetCourse(courseId, true);

            if (course.CourseTasks.Any(x => x.Name == name))
            {
                throw new CourseTaskAlreadyExistException(name);
            }
        }
    }

}
