using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer4.Validation;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduManagementLab.Api.Controllers
{
    [Route("LTIScores")]
    [ApiController]
    public class LTIScoresController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly CourseTaskService _courseTaskService;

        public LTIScoresController(CourseTaskService courseTaskService, CourseService courseService)
        {
            _courseTaskService = courseTaskService;
            _courseService = courseService;
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
        public ActionResult<Score> AddLTIScore(Guid courseId, Guid LTILineItemId, Guid memberId, double score)
        {
            var courseTaskResult = _courseService.GetCourse(courseId, true).CourseTasks.FirstOrDefault(c => c.Id == LTILineItemId).Results.FirstOrDefault(c => c.MembershipId == memberId);

            //var lineItemsResults = new List<Score>();
            Score scoreResult = new Score();
            scoreResult = new Score
            {
                UserId = courseTaskResult.MembershipId.ToString(),
                ScoreGiven = score,
                ScoreMaximum = 100,
                Comment = courseTaskResult.Score > 60 ? "Good job!" : "Work harder..",
                ActivityProgress = ActivityProgress.InProgress,
                GradingProgress = GradingProgess.Pending,
                TimeStamp = DateTime.Now
            };
            return scoreResult;
        }

        [HttpGet]
        [Produces(Constants.MediaTypes.Score)]
        [ProducesResponseType(typeof(Score), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.LtiScopes.Ags.ScoreReadonly + " " + Constants.LtiScopes.Ags.Score)]
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
