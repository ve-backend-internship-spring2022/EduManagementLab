using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer4.Validation;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduManagementLab.Api.Controllers
{
    [Route("LTILineItems")]
    [ApiController]
    public class LTILineItemsController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly CourseTaskService _courseTaskService;

        public LTILineItemsController(CourseTaskService courseTaskService, CourseService courseService)
        {
            _courseTaskService = courseTaskService;
            _courseService = courseService;
        }

        [HttpGet]
        [Produces(Constants.MediaTypes.LineItemContainer)]
        [ProducesResponseType(typeof(LineItemContainer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem + " " + Constants.LtiScopes.Ags.LineItemReadonly)]
        [Route("{courseId}/LTILineItems", Name = Constants.ServiceEndpoints.Ags.LineItemsService)]
        public ActionResult<LineItemContainer> GetLineItems(Guid courseId)
        {
            var courseTaskList = _courseService.GetCourse(courseId, true).CourseTasks;
            LineItemContainer newcontaint = new LineItemContainer();

            List<LineItem> lineItems = new List<LineItem>();
            foreach (var courseTask in courseTaskList)
            {
                LineItem lineItem = new LineItem()
                {
                    Id = $"https://localhost:7134/LTIResults/{courseId}/LTILineItem/{courseTask.Id}",
                    ScoreMaximum = 100.00,
                    StartDateTime = courseTask.LastUpdate,
                    Label = courseTask.Name,
                    Tag = "Score",
                    ResourceId = courseTask.IMSLTIResultResourceId.ToString(),
                };
                foreach (var resourceLink in courseTask.IMSLTIResourceLinks)
                {
                    lineItem.ResourceLinkId = resourceLink.Id.ToString();
                }
                lineItems.Add(lineItem);
            }
            newcontaint.AddRange(lineItems);

            return Ok(newcontaint);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem + " " + Constants.LtiScopes.Ags.LineItemReadonly)]
        [Route("{courseId}/LTILineItem/{LTILineItemId}", Name = Constants.ServiceEndpoints.Ags.LineItemService)]
        public ActionResult<LineItem> GetLineItem(Guid courseId, Guid LTILineItemId)
        {
            var courseTask = _courseService.GetCourse(courseId).CourseTasks.FirstOrDefault(c => c.Id == LTILineItemId);
            LineItem newLineItem = new LineItem()
            {
                Id = $"https://localhost:7134/LTIScores/{courseId}/LTILineItem/{courseTask.Id}",
                Label = courseTask.Name,
                ResourceId = courseTask.IMSLTIResultResourceId.ToString(),
            };
            return Ok(newLineItem);
        }

        [HttpPost]
        [Consumes(Constants.MediaTypes.LineItem)]
        [Produces(Constants.MediaTypes.LineItem)]
        [ProducesResponseType(typeof(LineItem), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem)]
        [Route("{courseId}/LTILineItems", Name = Constants.ServiceEndpoints.Ags.LineItemsService)]
        public ActionResult<LineItem> AddLTILineItem(Guid courseId, LineItem lineItem)
        {
            try
            {
                _courseTaskService.CreateCourseTask(courseId, lineItem.Label, "");
            }
            catch (CourseTaskAlreadyExistException ex)
            {
                return BadRequest(ex.Message);
            }
            return Created("", lineItem);
        }

        [HttpPut]
        [Consumes(Constants.MediaTypes.LineItem)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem)]
        [Route("{courseId}/LTILineItems/{LTILineItemId}")]
        public LineItem UpdateLineItem(Guid courseId, Guid LTILineItemId, LineItem lineItem)
        {
            var result = _courseTaskService.UpdateCourseTask(LTILineItemId, lineItem.Label, "");

            var updatedLineITem = new LineItem()
            {
                Id = result.Id.ToString(),
                Label = result.Name,
                ResourceId = result.IMSLTIResultResourceId.ToString(),
                StartDateTime = result.LastUpdate,
            };
            return updatedLineITem;
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem)]
        [Route("{courseId}/LTILineItems/{LTILineItemId}")]
        public LineItem DeleteLineItem(Guid courseId, Guid LTILineItemId)
        {
            var targetCourseTask = _courseTaskService.DeleteCourseTask(LTILineItemId);
            LineItem deletedLineItem = new LineItem()
            {
                Id = targetCourseTask.Id.ToString(),
                Label = targetCourseTask.Name,
                StartDateTime = targetCourseTask.LastUpdate,
                ResourceId = targetCourseTask.IMSLTIResultResourceId.ToString(),
            };
            return deletedLineItem;
        }
    }
}
