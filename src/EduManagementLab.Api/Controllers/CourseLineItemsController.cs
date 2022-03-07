using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace EduManagementLab.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CourseLineItemsController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly CourseLineItemService _courseLineItemService;

        public CourseLineItemsController(CourseLineItemService courseLineItemService, CourseService courseService)
        {
            _courseLineItemService = courseLineItemService;
            _courseService = courseService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<CourseLineItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}/LineItems")]
        public ActionResult<IEnumerable<CourseLineItem>> GetCourseLineItems(Guid courseId)
        {
            try
            {
                var lineItemList = _courseService.GetCourse(courseId, false).CourseLineItems.ToList();

                return Ok(lineItemList);
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(CourseLineItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}")]
        public ActionResult<CourseLineItem> GetCourseLineItem(Guid courseLineItemId)
        {
            try
            {
                var courseLineItem = _courseLineItemService.GetCourseLineItem(courseLineItemId);
                return Ok(courseLineItem);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CourseLineItem), StatusCodes.Status201Created)]
        public ActionResult<CourseLineItem> AddCourseLineItem(Guid courseId, string lineItemName, string lineItemDescription, bool lineItemActive)
        {
            var newCourseLineItem = _courseLineItemService.CreateCourseLineItem(courseId, lineItemName, lineItemDescription, lineItemActive);
            return CreatedAtAction(nameof(GetCourseLineItem), new { lineItemId = newCourseLineItem.Id }, newCourseLineItem);
        }

        [HttpPatch()]
        [ProducesResponseType(typeof(CourseLineItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/UpdateLineItemInfo")]
        public ActionResult<CourseLineItem> UpdateCourseLineItemInfo(Guid lineItemId, string lineItemName, string lineItemDescription)
        {
            try
            {
                var newCourseLineItem = _courseLineItemService.UpdateCourseLineItemInfo(lineItemId, lineItemName, lineItemDescription);
                return Ok(newCourseLineItem);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPatch()]
        [ProducesResponseType(typeof(CourseLineItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/UpdateLineItemActive")]
        public ActionResult<CourseLineItem> UpdateCourseLineItemActive(Guid lineItemId, bool lineItemActive)
        {
            try
            {
                var newCourseLineItem = _courseLineItemService.UpdateCourseLineItemActive(lineItemId, lineItemActive);
                return Ok(newCourseLineItem);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}")]
        public ActionResult DeleteCourseLineItem(Guid courseLineItemId)
        {
            try
            {
                _courseLineItemService.DeleteCourseLineItem(courseLineItemId);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<CourseLineItem.Result>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/Results")]
        public ActionResult<IEnumerable<CourseLineItem.Result>> GetLineItemResults(Guid lineItemId)
        {
            try
            {
                var resultsList = _courseLineItemService.GetCourseLineItem(lineItemId).Results.ToList();

                return Ok(resultsList);
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<CourseLineItem.Result>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/Results/{userId}")]
        public ActionResult<CourseLineItem.Result> GetLineItemResult(Guid lineItemId, Guid userId)
        {
            try
            {
                var resultList = _courseLineItemService.GetCourseLineItem(lineItemId).Results.ToList();

                if (resultList.Any(u => u.UserId == userId))
                {
                    var selectedMembership = resultList.FirstOrDefault(u => u.UserId == userId);

                    return Ok(selectedMembership);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{lineItemId}/Results")]
        [ProducesResponseType(typeof(CourseLineItem.Result), StatusCodes.Status201Created)]
        public ActionResult<CourseLineItem.Result> AddLineItemResult(Guid lineItemId, Guid userId, decimal score)
        {
            var newlineItemResult = _courseLineItemService.CreateLineItemResult(lineItemId, userId, score);
            return CreatedAtAction(nameof(GetLineItemResult), new { lineItemId = newlineItemResult.CourseLineItemId, userId = newlineItemResult.UserId }, newlineItemResult);
        }

        [HttpPut]
        [ProducesResponseType(typeof(CourseLineItem.Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/Results")]
        public ActionResult<CourseLineItem.Result> UpdateLineItemResult(Guid lineItemId, Guid userId, decimal score)
        {
            try
            {
                var newLineItemResult = _courseLineItemService.UpdateLineItemResult(lineItemId, userId, score);
                return Ok(newLineItemResult);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(CourseLineItem.Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/Results/{membershipId}")]
        public ActionResult<CourseLineItem.Result> DeleteLineItemResult(Guid lineItemId, Guid membershipId)
        {
            try
            {
                _courseLineItemService.DeleteLineItemResult(lineItemId, membershipId);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
