using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Services
{
    public class CourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Course> GetCourses()
        {
            return _unitOfWork.Courses.GetAll();
        }

        public Course CreateCourse(string code, string name, string description, DateTime startDate, DateTime endDate)
        {
            var course = new Course() { Code = code, Name = name, Description = description, StartDate = startDate, EndDate = endDate };
            _unitOfWork.Courses.Add(course);
            _unitOfWork.Complete();
            return course;
        }

        public Course GetCourse(Guid id, bool includeMembershipUsers = false)
        {
            var course = _unitOfWork.Courses.GetCourse(id, includeMembershipUsers);
            if (course == null)
            {
                throw new CourseNotFoundException(id);
            }
            return course;
        }
        public Course UpdateCourseInfo(Guid id, string code, string name, string description)
        {
            var course = GetCourse(id);
            course.Code = code;
            course.Name = name;
            course.Description = description;
            _unitOfWork.Courses.Update(course);
            _unitOfWork.Complete();
            return course;
        }

        public Course UpdateCoursePeriod(Guid id, DateTime startDate, DateTime endDate)
        {
            var course = GetCourse(id);
            course.StartDate = startDate;
            course.EndDate = endDate;
            _unitOfWork.Courses.Update(course);
            _unitOfWork.Complete();
            return course;
        }

        public void DeleteCourse(Guid id)
        {
            var course = GetCourse(id);
            _unitOfWork.Courses.Remove(course);
            _unitOfWork.Complete();
        }

        public Course CreateCourseMembership(Guid courseId, Guid userId, DateTime enrolledDate)
        {
            var course = GetCourse(courseId, true);

            if (!course.Memperships.Any(c => c.UserId == userId))
            {
                course.Memperships.Add(new Course.Membership()
                {
                    CourseId = courseId,
                    UserId = userId,
                    EnrolledDate = enrolledDate
                });
            }
            _unitOfWork.Courses.Update(course);
            _unitOfWork.Complete();
            return course;
        }
        public Course RemoveCourseMembership(Guid courseId, Guid userId)
        {
            var course = _unitOfWork.Courses.GetCourse(courseId, true);
            if (course.Memperships.Any(c => c.UserId == userId && c.CourseId == courseId))
            {
                var membership = course.Memperships.Find(c => c.UserId == userId && c.CourseId == courseId);
                course.Memperships.Remove(membership);
            }
            _unitOfWork.Courses.Update(course);
            _unitOfWork.Complete();
            return course;
        }

        public Course UpdateMembershipEnrolledDate(Guid courseId, Guid userId, DateTime enrolledDate)
        {
            var course = GetCourse(courseId, true);

            if (course.Memperships.Any(c => c.UserId == userId && c.CourseId == courseId))
            {
                var membership = course.Memperships.FirstOrDefault(u => u.UserId == userId && u.CourseId == courseId);
                membership.EnrolledDate = enrolledDate;
            }
            _unitOfWork.Courses.Update(course);
            _unitOfWork.Complete();
            return course;
        }
    }
}
