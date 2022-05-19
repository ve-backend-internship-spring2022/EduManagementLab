using EduManagementLab.Core.Services;
using LtiAdvantage;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [Produces(Constants.MediaTypes.ResultContainer)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.ResultReadonly)]
        [Route("{courseId}/LTILineItem/{LTILineItemId}/results", Name = Constants.ServiceEndpoints.Ags.ResultsService)]
        public ActionResult<ResultContainer> GetLTIResults(Guid courseId, Guid LTILineItemId)
        {
            var courseTaskResultList = _courseService.GetCourse(courseId, true).CourseTasks.FirstOrDefault(c => c.Id == LTILineItemId).Results;
            var lineItemsResults = new ResultContainer();

            foreach (var courseTaskResult in courseTaskResultList)
            {
                lineItemsResults.Add(new Result
                {
                    Id = courseTaskResult.Id.ToString(),
                    Comment = "This is comment",
                    ResultMaximum = 100,
                    ResultScore = (double)courseTaskResult.Score,
                    UserId = courseTaskResult.Membership.UserId.ToString(),
                    ScoreOf = $"https://localhost:7134/LTILineItems/{courseId}/LTILineItem/{courseTaskResult.Id}",
                });
            }
            return Ok(lineItemsResults);
        }

        [HttpPost]
        [Consumes(Constants.MediaTypes.Score)]
        [Produces(Constants.MediaTypes.Score)]
        [ProducesResponseType(typeof(Score), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.Score)]
        [Route("{courseId}/LTILineItem/{LTILineItemId}/scores", Name = Constants.ServiceEndpoints.Ags.ScoresService)]
        public void AddLTIScore(Guid courseId, Guid LTILineItemId, [FromBody] string score)
        {
            Score scores = JsonConvert.DeserializeObject<Score>(score);

            var memberId = _courseService.GetCourse(courseId, true).Memperships.FirstOrDefault(u => u.UserId == Guid.Parse(scores.UserId)).Id;

            if (_courseService.GetCourse(courseId, true).CourseTasks.FirstOrDefault(c => c.Id == LTILineItemId).Results.Any(c => c.MembershipId == memberId))
            {
                _courseTaskService.UpdateCourseTaskResult(LTILineItemId, memberId, (decimal)scores.ScoreGiven);
            }
            else
            {
                _courseTaskService.CreateCourseTaskResult(LTILineItemId, memberId, (decimal)scores.ScoreGiven);
            }
        }

        [HttpGet]
        [Produces(Constants.MediaTypes.Score)]
        [ProducesResponseType(typeof(Score), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Ags.ScoreReadonly + " " + Constants.LtiScopes.Ags.Score)]
        [Route("{courseId}/LTILineItem/{LTILineItemId}/scores/{scoreId}")]
        public ActionResult<Score> GetLTIScore(Guid courseId, Guid LTILineItemId, Guid memberId)
        {
            var courseTaskResult = _courseService.GetCourse(courseId, true).CourseTasks.FirstOrDefault(c => c.Id == LTILineItemId).Results.FirstOrDefault(c => c.MembershipId == memberId);

            return new Score
            {
                UserId = courseTaskResult.MembershipId.ToString(),
                ScoreGiven = (double)courseTaskResult.Score,
                ScoreMaximum = 100,
                Comment = courseTaskResult.Score > 60 ? "Good job!" : "Work harder..",
                ActivityProgress = ActivityProgress.InProgress,
                GradingProgress = GradingProgess.Pending,
                TimeStamp = DateTime.Now
            };
        }

    }
}
