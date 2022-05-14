using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer4.Validation;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduManagementLab.Api.Controllers
{
    [Route("LTIResults")]
    [ApiController]
    public class LTIResultsController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly CourseTaskService _courseTaskService;

        public LTIResultsController(CourseTaskService courseTaskService, CourseService courseService)
        {
            _courseTaskService = courseTaskService;
            _courseService = courseService;
        }

        [HttpGet]
        //[Produces(Constants.MediaTypes.ResultContainer)]
        //[ProducesResponseType(typeof(ResultContainer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.ResultReadonly)]
        [Route("{courseId}/LTILineItem/{LTILineItemId}/results", Name = Constants.ServiceEndpoints.Ags.ResultsService)]
        public IEnumerable<Result> GetLTIResults(Guid courseId, Guid LTILineItemId)
        {
            var courseTaskResultList = _courseService.GetCourse(courseId, true).CourseTasks.FirstOrDefault(c => c.Id == LTILineItemId).Results;

            var lineItemsResults = new List<Result>();
            foreach (var courseTaskResult in courseTaskResultList)
            {
                //var coursetaskresult = _courseTaskService.GetCourseTask(courseTaskResult.Id, true, true).Results;
                lineItemsResults.Add(new Result
                {
                    Id = courseTaskResult.Id.ToString(),
                    Comment = "This is comment",
                    ResultMaximum = 100,
                    ResultScore = (double)courseTaskResult.Score,
                    UserId = courseTaskResult.MembershipId.ToString(),
                    ScoreOf = $"https://localhost:7134/LTILineItems/{courseId}/LTILineItem/{courseTaskResult.Id}"
                });
            }
            return lineItemsResults;
        }
    }
}
