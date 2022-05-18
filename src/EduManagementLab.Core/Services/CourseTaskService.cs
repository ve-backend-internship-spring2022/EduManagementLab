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
    public partial class CourseTaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseTaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CourseTask> GetCourseTasks()
        {
            return _unitOfWork.CourseTasks.GetAll();
        }
        public IEnumerable<CourseTask> GetCourseTasksIncludeResourceLinks()
        {
            return _unitOfWork.CourseTasks.GetCourseTasks(true, true);
        }
        public CourseTask GetCourseTask(Guid courseTaskId, bool includeResults = false, bool includeResourceLinks = true)
        {
            var courseTask = _unitOfWork.CourseTasks.GetCourseTask(courseTaskId, includeResults, includeResourceLinks);
            if (courseTask == null)
            {
                throw new CourseTaskNotFoundException(courseTaskId);
            }
            return courseTask;
        }
        public CourseTask CreateCourseTask(Guid courseId, string name, string description)
        {
            var course = _unitOfWork.Courses.GetCourse(courseId, true);

            Guard.AgainstNullCourse(courseId, _unitOfWork);
            Guard.AgaintDuplicateNameInCourseTask(courseId, name, _unitOfWork);

            CourseTask newCourseTask = new CourseTask()
            {
                Name = name,
                Description = description,
                LastUpdate = DateTime.Now,
            };
            course.CourseTasks.Add(newCourseTask);

            _unitOfWork.CourseTasks.Add(newCourseTask);
            _unitOfWork.Complete();
            return newCourseTask;

        }
        public CourseTask UpdateCourseTask(Guid courseTaskId, string name, string description)
        {
            CourseTask courseTask = GetCourseTask(courseTaskId);
            courseTask.Name = name;
            courseTask.Description = description;

            _unitOfWork.CourseTasks.Update(courseTask);
            _unitOfWork.Complete();
            return courseTask;
        }

        public CourseTask AddResouceLinkToCourseTask(Guid courseTaskId, IMSLTIResourceLink resourceLink)
        {
            var targetCourseTask = GetCourseTask(courseTaskId, true, true);

            if (!targetCourseTask.IMSLTIResourceLinks.Any(r => r.Title == resourceLink.Title))
            {
                targetCourseTask.IMSLTIResourceLinks.Add(resourceLink);

                _unitOfWork.CourseTasks.Update(targetCourseTask);
                _unitOfWork.Complete();
            }
            return targetCourseTask;
        }
        public CourseTask DeleteCourseTaskResoruceLink(Guid courseTaskId, Guid resourceId)
        {
            var targetCourseTask = GetCourseTask(courseTaskId, true, true);

            if (targetCourseTask.IMSLTIResourceLinks.Any(r => r.Id == resourceId))
            {
                var targetResourceLink = _unitOfWork.ResourceLinks.GetById(resourceId);
                targetCourseTask.IMSLTIResourceLinks.Remove(targetResourceLink);

                _unitOfWork.CourseTasks.Update(targetCourseTask);
                _unitOfWork.Complete();
            }
            return targetCourseTask;
        }

        public CourseTask DeleteCourseTask(Guid courseTaskId)
        {
            var courseTask = GetCourseTask(courseTaskId);

            Guard.AgainstUnknownCourseTask(courseTaskId, _unitOfWork);

            _unitOfWork.CourseTasks.Remove(courseTask);
            _unitOfWork.Complete();
            return courseTask;
        }
        public CourseTask.Result UpdateCourseTaskResult(Guid courseTaskId, Guid memberId, decimal score)
        {
            var courseTask = GetCourseTask(courseTaskId, true);

            //Guard.AgainstUnknownCourseLineItem(lineItemId, _unitOfWork);
            Guard.AgainstUnknownMemberInCourseTaskResult(courseTaskId, memberId, _unitOfWork);

            var result = courseTask.Results.FirstOrDefault(l => l.MembershipId == memberId && l.CourseTaskId == courseTaskId);

            result.Score = score;
            result.LastUpdated = DateTime.Now;

            _unitOfWork.CourseTaskResults.Update(result);
            _unitOfWork.Complete();
            return result;
        }
        public CourseTask.Result CreateCourseTaskResult(Guid courseTaskId, Guid memberId, decimal score)
        {
            var courseTask = GetCourseTask(courseTaskId, true);

            Guard.AgainstDuplicateCourseTaskResult(courseTaskId, memberId, _unitOfWork);

            CourseTask.Result newResult = null;

            if (!courseTask.Results.Any(x => x.MembershipId == memberId && x.CourseTaskId == courseTaskId))
            {
                newResult = new CourseTask.Result()
                {
                    CourseTaskId = courseTaskId,
                    MembershipId = memberId,
                    Score = score,
                    LastUpdated = DateTime.Now
                };

                courseTask.Results.Add(newResult);

                _unitOfWork.CourseTaskResults.Add(newResult);
            }

            _unitOfWork.Complete();
            return newResult;
        }
        public CourseTask.Result DeleteCourseTaskResult(Guid courseTaskId, Guid memberId)
        {
            var courseTask = GetCourseTask(courseTaskId, true);

            Guard.AgainstUnknownMemberInCourseTaskResult(courseTaskId, memberId, _unitOfWork);

            var resultToDelete = courseTask.Results.FirstOrDefault(l => l.MembershipId == memberId && l.CourseTaskId == courseTaskId);
            _unitOfWork.CourseTaskResults.Remove(resultToDelete);
            _unitOfWork.Complete();
            return resultToDelete;
        }
    }
}
