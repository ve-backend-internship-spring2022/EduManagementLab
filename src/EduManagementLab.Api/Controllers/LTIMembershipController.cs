using EduManagementLab.Core.Services;
using LtiAdvantage;
using LtiAdvantage.NamesRoleProvisioningService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduManagementLab.Api.Controllers
{
    [Route("LTIMemberships")]
    [ApiController]
    public class LTIMembershipController : ControllerBase
    {
        private readonly CourseService _courseService;
        public LTIMembershipController(CourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [Produces(Constants.MediaTypes.MembershipContainer)]
        [ProducesResponseType(typeof(MembershipContainer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.LtiScopes.Nrps.MembershipReadonly)]
        [Route("{courseId}/LTIMembership", Name = Constants.ServiceEndpoints.Nrps.MembershipService)]
        public MembershipContainer GetMembership(Guid courseId)
        {
            var targetMemberships = _courseService.GetCourse(courseId, true).Memperships;
            var targetCourse = targetMemberships.FirstOrDefault(c => c.Course.Id == courseId).Course;

            Context newContext = new Context()
            {
                Id = targetCourse.Id.ToString(),
                Title = targetCourse.Name,
                Label = targetCourse.Name
            };

            List<Member> memberlist = new List<Member>();
            foreach (var member in targetMemberships)
            {
                Member newMember = new Member()
                {
                    LisPersonSourcedId = member.User.Id.ToString(),
                    Lti11LegacyUserId = null,
                    Picture = null,
                    Roles = null,
                    Message = null,
                    Email = member.User.Email,
                    Name = $"{member.User.FirstName} {member.User.LastName}",
                    UserId = member.User.Id.ToString(),
                    GivenName = member.User.FirstName,
                    FamilyName = member.User.LastName,
                    Status = MemberStatus.Active
                };
                memberlist.Add(newMember);
            };

            MembershipContainer membershipContainer = new MembershipContainer();
            membershipContainer.Context = newContext;
            membershipContainer.Id = Constants.ServiceEndpoints.Nrps.MembershipService;
            membershipContainer.Members = memberlist;

            return membershipContainer;
        }
    }
}
