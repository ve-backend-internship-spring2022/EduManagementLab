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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.LtiScopes.Ags.LineItem + " " + Constants.LtiScopes.Ags.LineItemReadonly)]
        [Route("{courseId}/LTILineItems", Name = Constants.ServiceEndpoints.Ags.LineItemsService)]
        public IEnumerable<LineItem> GetLineItems(Guid courseId)
        {
            var courseTaskList = _courseService.GetCourse(courseId, true).CourseTasks;

            List<LineItem> lineItems = new List<LineItem>();
            foreach (var courseTask in courseTaskList)
            {
                //var coursetaskresult = _courseTaskService.GetCourseTask(courseTask.Id, true, true).Results;
                lineItems.Add(new LineItem
                {
                    Id = courseTask.Id.ToString(),
                    ScoreMaximum = 100.00,
                    StartDateTime = courseTask.LastUpdate,
                    Label = courseTask.Name,
                    Tag = "Score",
                    ResourceId = courseTask.IMSLTIResultResourceId.ToString(),
                    ResourceLinkId = null,
                });
            }

            return lineItems;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.LtiScopes.Ags.LineItem + " " + Constants.LtiScopes.Ags.LineItemReadonly)]
        [Route("{courseId}/LTILineItem/{LTILineItemId}", Name = Constants.ServiceEndpoints.Ags.LineItemService)]
        public LineItem GetLineItem(Guid courseId, Guid LTILineItemId)
        {
            var courseTask = _courseService.GetCourse(courseId).CourseTasks.FirstOrDefault(c => c.Id == LTILineItemId);
            LineItem newLineItem = new LineItem()
            {
                Id = courseTask.Id.ToString(),
                Label = courseTask.Name,
                ResourceId = courseTask.IMSLTIResultResourceId.ToString(),
            };
            return newLineItem;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem)]
        [Route("{courseId}/LTILineItem")]
        public LineItem AddLTILineItem(Guid courseId, LineItem lineItem)
        {
            _courseTaskService.CreateCourseTask(courseId, lineItem.Label, "");
            return lineItem;
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem)]
        [Route("{courseId}/LTILineItem/{LTILineItemId}")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.LineItem)]
        [Route("{courseId}/LTILineItem/{LTILineItemId}")]
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
