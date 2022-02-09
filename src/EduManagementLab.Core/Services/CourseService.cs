using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Interfaces;
using EduManagementLab.Core.Interfaces.Repositories;
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
            var allCourses = GetCourses();
            if (allCourses.Any(x => x.Code == code || x.Name == name))
            {
                throw new CourseAlreadyExistException();
            }
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
        public IEnumerable<Course.Membership> GetUserCourses(Guid userId)
        {
            return _unitOfWork.Courses.GetUserCourses(userId);
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

        public Course.Membership CreateCourseMembership(Guid courseId, Guid userId, DateTime enrolledDate)
        {
            var course = GetCourse(courseId, true);
            var user = _unitOfWork.Users.GetById(userId);

            if(course != null)
            {
                if(user != null)
                {
                    if (!course.Memperships.Any(c => c.UserId == userId))
                    {
                        Course.Membership newMembership = new Course.Membership()
                        {
                            CourseId = courseId,
                            UserId = userId,
                            EnrolledDate = enrolledDate,
                        };

                        course.Memperships.Add(newMembership);

                        _unitOfWork.Courses.Update(course);
                        _unitOfWork.Complete();
                        return newMembership;
                    }
                    throw new MembershipAlreadyExistException();
                }
                throw new CourseNotFoundException(userId);
            }
            throw new UserNotFoundException(courseId);
        }

    
        public Course.Membership RemoveCourseMembership(Guid courseId, Guid userId)
        {
            var course = _unitOfWork.Courses.GetCourse(courseId, true);

            if (course.Memperships.Any(c => c.CourseId == courseId))
            {
                if (course.Memperships.Any(c => c.UserId == userId))
                {
                    var membershipToDelete = course.Memperships.Find(c => c.UserId == userId && c.CourseId == courseId);
                    course.Memperships.Remove(membershipToDelete);

                    _unitOfWork.Courses.Update(course);
                    _unitOfWork.Complete();
                    return membershipToDelete;
                }
                throw new CourseNotFoundException(userId);
            }
            throw new UserNotFoundException(courseId);
        }

        public Course.Membership UpdateMembershipEnrolledDate(Guid courseId, Guid userId, DateTime enrolledDate)
        {
            var course = GetCourse(courseId, true);

            if (course.Memperships.Any(c => c.CourseId == courseId))
            {
                if (course.Memperships.Any(c => c.UserId == userId))
                {
                    var membershipToUpdate = course.Memperships.FirstOrDefault(u => u.UserId == userId && u.CourseId == courseId);
                    membershipToUpdate.EnrolledDate = enrolledDate;

                    _unitOfWork.Courses.Update(course);
                    _unitOfWork.Complete();
                    return membershipToUpdate;
                }
                throw new CourseNotFoundException(userId);
            }
            throw new UserNotFoundException(courseId);
        }
    }
}
