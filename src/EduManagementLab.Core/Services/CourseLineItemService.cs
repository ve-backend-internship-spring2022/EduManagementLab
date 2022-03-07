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
        public CourseLineItem GetCourseLineItem(Guid id)
        {
            var courseLineItem = _unitOfWork.CourseLineItems.GetById(id);
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

        //public IEnumerable<CourseLineItem.LineItemResult> GetLineItemResults()
        //{
        //    return _unitOfWork.LineItemResults.GetAll();
        //}
        //public CourseLineItem.LineItemResult GetLineItemResult(Guid id)
        //{
        //    var lineItemResult = _unitOfWork.LineItemResults.GetById(id);
        //    if (lineItemResult == null)
        //    {
        //        throw new LineItemResultNotFoundException(id);
        //    }
        //    return lineItemResult;
        //}
        public IEnumerable<CourseLineItem.Result> GetLineItemResults(Guid lineItemId)
        {
            var resultsList = _unitOfWork.CourseLineItems.GetCourseLineItem(lineItemId, true).Results.ToList();

            return resultsList; 
        }
        public CourseLineItem.Result CreateLineItemResult(Guid lineItemId, Guid userId, decimal score)
        {
            var lineItem = GetCourseLineItem(lineItemId);

            CourseLineItem.Result newResult = new CourseLineItem.Result()
            {
                CourseLineItemId = lineItemId,
                UserId = userId,
                Score = score,
            };

            lineItem.Results.Add(newResult);

            _unitOfWork.CourseLineItems.Update(lineItem);
            _unitOfWork.Complete();
            return newResult;
        }


        public CourseLineItem.Result UpdateLineItemResult(Guid lineItemId, Guid userId, decimal score)
        {

            var lineItem = GetCourseLineItem(lineItemId);

            var resultToUpdate = lineItem.Results.FirstOrDefault(l => l.UserId == userId && l.CourseLineItemId == lineItemId);

            resultToUpdate.Score = score;

            _unitOfWork.CourseLineItems.Update(lineItem);
            _unitOfWork.Complete();
            return resultToUpdate;
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
