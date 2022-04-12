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

        public static void AgainstDuplicateCourseLineItemResult(Guid lineItemId, Guid memberId, IUnitOfWork unitOfWork)
        {
            var courseLineItem = unitOfWork.CourseLineItems.GetCourseLineItem(lineItemId, true);

            if(courseLineItem.Results.Any(c => c.MembershipId == memberId && c.CourseLineItemId == lineItemId))
            {
                throw new MemberInCourseLineItemResultAlreadyExistException(memberId);
            }
        }

        public static void AgainstUnknownMemberInCourseLineItemResult(Guid lineItemId, Guid memberId, IUnitOfWork unitOfWork)
        {
            var courseLineItem = unitOfWork.CourseLineItems.GetCourseLineItem(lineItemId, true);

            if (!courseLineItem.Results.Any(c => c.MembershipId == memberId && c.CourseLineItemId == lineItemId))
            {
                throw new MemberInCourseLineItemResultNotFoundException(memberId);
            }
        }

        public static void AgainstUnknownCourseLineItem(Guid lineItemId, IUnitOfWork unitOfWork)
        {
            var courseLineItem = unitOfWork.CourseLineItems.GetCourseLineItem(lineItemId, true);

            if (!courseLineItem.Results.Any(x => x.CourseLineItemId == lineItemId))
            {
                throw new CourseLineItemNotFoundException(lineItemId);
            }
        }

        public static void AgaintDuplicateNameInCourseLineItem(Guid courseId, string name, IUnitOfWork unitOfWork)
        {
            var course = unitOfWork.Courses.GetCourse(courseId, true);

            if (course.CourseLineItems.Any(x => x.Name == name))
            {
                throw new CourseLineItemAlreadyExistException(name);
            }
        }
    }

}
