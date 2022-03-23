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
    public partial class CourseLineItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseLineItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CourseLineItem> GetCourseLineItems()
        {
            return _unitOfWork.CourseLineItems.GetAll();
        }
        public CourseLineItem GetCourseLineItem(Guid id, bool includeResults = false)
        {
            var courseLineItem = _unitOfWork.CourseLineItems.GetCourseLineItem(id, includeResults);
            if (courseLineItem == null)
            {
                throw new CourseLineItemNotFoundException(id);
            }
            return courseLineItem;
        }
        public CourseLineItem CreateCourseLineItem(Guid courseId, string name, string description, bool active)
        {
            var course = _unitOfWork.Courses.GetCourse(courseId, false);

            CourseLineItem newlineItem = new CourseLineItem()
            {
                Name = name,
                Description = description,
                Active = active
            };

            course.CourseLineItems.Add(newlineItem);

            _unitOfWork.CourseLineItems.Add(newlineItem);
            _unitOfWork.Complete();
            return newlineItem;

        }

        public CourseLineItem UpdateCourseLineItemInfo(Guid id, string name, string description)
        {
            CourseLineItem courseLineItem = GetCourseLineItem(id);
            courseLineItem.Name = name;
            courseLineItem.Description = description;

            _unitOfWork.CourseLineItems.Update(courseLineItem);
            _unitOfWork.Complete();
            return courseLineItem;
        }

        public CourseLineItem UpdateCourseLineItemActive(Guid id, bool active)
        {
            CourseLineItem courseLineItem = GetCourseLineItem(id);
            courseLineItem.Active = active;

            _unitOfWork.CourseLineItems.Update(courseLineItem);
            _unitOfWork.Complete();
            return courseLineItem;
        }

        public void DeleteCourseLineItem(Guid id)
        {
            var courseLineItem = GetCourseLineItem(id);
            _unitOfWork.CourseLineItems.Remove(courseLineItem);
            _unitOfWork.Complete();
        }

        public CourseLineItem.Result CreateLineItemResult(Guid lineItemId, Guid userId, decimal score)
        {
            var courseLineItem = GetCourseLineItem(lineItemId, true);

            CourseLineItem.Result newResult = null;

            if (!courseLineItem.Results.Any(x => x.UserId == userId))
            {
                newResult = new CourseLineItem.Result()
                {
                    CourseLineItemId = lineItemId,
                    UserId = userId,
                    Score = score,
                };

                courseLineItem.Results.Add(newResult);
            }
            //_unitOfWork.CourseLineItems.Update(courseLineItem);
            _unitOfWork.LineItemResults.Add(newResult);
            _unitOfWork.Complete();
            return newResult;
        }

        public CourseLineItem.Result UpdateLineItemResult(Guid lineItemId, Guid userId, decimal score)
        {           
            var lineItem = GetCourseLineItem(lineItemId, true);

            var result = lineItem.Results.FirstOrDefault(l => l.UserId == userId && l.CourseLineItemId == lineItemId);

            result.Score = score;

            _unitOfWork.LineItemResults.Update(result);
            _unitOfWork.Complete();
            return result;
        }

        public CourseLineItem.Result DeleteLineItemResult(Guid lineItemId, Guid userId)
        {
            var lineItem = GetCourseLineItem(lineItemId);

            var resultToDelete = lineItem.Results.Find(l => l.UserId == userId && l.CourseLineItemId == lineItemId);

            lineItem.Results.Remove(resultToDelete);

            _unitOfWork.CourseLineItems.Update(lineItem);
            _unitOfWork.Complete();
            return resultToDelete;
        }
    }
}
