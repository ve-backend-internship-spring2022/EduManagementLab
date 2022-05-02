using EduManagementLab.Core.Configuration;
using EduManagementLab.Core.Entities;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using LtiAdvantage;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.DeepLinking;
using LtiAdvantage.Lti;
using LtiAdvantage.NamesRoleProvisioningService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Security.Claims;

namespace EduManagementLab.Core.Services
{
    public class CustomProfileService : IProfileService
    {
        protected readonly ILogger _logger;

        private readonly UserService _userService;
        private readonly CourseService _courseService;
        private readonly ToolService _toolService;
        private readonly ResourceLinkService _resourceSeService;
        private readonly CourseLineItemService _courseLineItemSeService;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomProfileService(UserService userService, ToolService toolService, LinkGenerator linkGenerator, CourseLineItemService courseLineItemService, ResourceLinkService resourceLinkService, CourseService courseService, ILogger<CustomProfileService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _courseService = courseService;
            _resourceSeService = resourceLinkService;
            _courseLineItemSeService = courseLineItemService;
            _linkGenerator = linkGenerator;
            _toolService = toolService;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.ValidatedRequest is ValidatedAuthorizeRequest request)
            {
                var sub = context.Subject.GetSubjectId();

                _logger.LogDebug("Getting LTI Advantage claims for identity token for subject: {subject} and client: {clientId}",
                    context.Subject.GetSubjectId(),
                    request.Client.ClientId);

                // LTI Advantage authorization requests include an lti_message_hint parameter
                var ltiMessageHint = request.Raw["lti_message_hint"];
                if (ltiMessageHint.IsMissing())
                {
                    _logger.LogInformation("Not an LTI request.");
                    return;
                }

                // LTI Advantage authorization requests include the the user id in the LoginHint
                // (also available in the Subject). In this sample platform, the user id is for one
                // of the tenants' people.
                string personId = request.LoginHint;
                if (personId == "")
                {
                    _logger.LogError("Cannot convert login hint to person id.");
                }

                // In this sample platform, the lti_message_hint is a JSON object that includes the
                // message type (LtiResourceLinkRequest or DeepLinkingRequest), the tenant's course
                // id, and either the resource link id or the tool id depending on the type of message.
                // For example, "{"id":3,"messageType":"LtiResourceLinkRequest","courseId":"1"}"
                var message = JToken.Parse(ltiMessageHint);
                var id = message.Value<string>("id");
                // In this sample platform, each application user is a tenant.
                var targetUser = _userService.GetUser(Guid.Parse(_httpContextAccessor.HttpContext.User.GetSubjectId()));
                var membership = _courseService.GetUserCourses(targetUser.Id);
                //var targetCourse = _courseService.GetUserCourses(targetUser.Id).Select(c => c.Course);
                var course = message.Value<string?>("courseId").Any() ? membership.FirstOrDefault(c => c.UserId == targetUser.Id).Course : null;
                var person = targetUser;

                var messageType = message.Value<string>("messageType");

                switch (messageType)
                {
                    case Constants.Lti.LtiResourceLinkRequestMessageType:
                        {
                            var resourceLink = _resourceSeService.GetResourceLink(Guid.Parse(id));

                            // Null unless there is exactly one gradebook column for the resource link.
                            var member = membership.FirstOrDefault(c => c.UserId == targetUser.Id);
                            var courseLineItems = _courseService.GetCourse(member.CourseId, true).CourseLineItems;
                            var courseLineItem = courseLineItems.FirstOrDefault(c => c.IMSLTIResourceLinks.Any(c => c.Id == Guid.Parse(id)));


                            context.IssuedClaims = GetResourceLinkRequestClaims(resourceLink, courseLineItem, person, course);

                            break;
                        }
                    case Constants.Lti.LtiDeepLinkingRequestMessageType:
                        {
                            var tool = _toolService.GetTool(Guid.Parse(id));

                            context.IssuedClaims = GetDeepLinkingRequestClaims(tool, person, course);

                            break;
                        }
                    default:
                        _logger.LogError($"{nameof(messageType)}=\"{messageType}\" not supported.");

                        break;
                }
            }
        }
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = _userService.GetUser(Guid.Parse(context.Subject.GetSubjectId()));
            context.IsActive = user != null;
        }
        private List<Claim> GetResourceLinkRequestClaims(IMSLTIResourceLink resourceLink, CourseLineItem gradebookColumn, User person, Course course)
        {
            var httpRequest = _httpContextAccessor.HttpContext.Request;

            var request = new LtiResourceLinkRequest
            {
                DeploymentId = resourceLink.Tool.DeploymentId,
                FamilyName = person.LastName,
                GivenName = person.FirstName,
                LaunchPresentation = new LaunchPresentationClaimValueType
                {
                    DocumentTarget = DocumentTarget.Window,
                    Locale = CultureInfo.CurrentUICulture.Name,
                    ReturnUrl = $"{httpRequest.Scheme}://{httpRequest.Host}"
                },
                Lis = new LisClaimValueType
                {
                    PersonSourcedId = person.Id.ToString(),
                    CourseSectionSourcedId = course?.Id.ToString()
                },
                Lti11LegacyUserId = person.Id.ToString(),
                Platform = new PlatformClaimValueType
                {
                    ContactEmail = "edulab@email.com",
                    Description = "Implementing LTI in Edu platform",
                    Guid = "localhost:5001",
                    Name = "EduLabPlatform",
                    ProductFamilyCode = "LTI Advantage",
                    Url = "https://localhost:5001",
                    Version = "LTI Specification 1.3"
                },
                ResourceLink = new ResourceLinkClaimValueType
                {
                    Id = resourceLink.Id.ToString(),
                    Title = resourceLink.Title,
                    Description = resourceLink.Description
                },
                Roles = ParsePersonRoles("ContextAdministrator, Administrator"),
                TargetLinkUri = resourceLink.Tool.LaunchUrl
            };

            // Add the context if the launch is from a course.
            if (course == null)
            {
                // Remove context roles
                request.Roles = request.Roles.Where(r => !r.ToString().StartsWith("Context")).ToArray();
            }
            else
            {
                request.Context = new ContextClaimValueType
                {
                    Id = course.Id.ToString(),
                    Title = course.Name,
                    Type = new[] { ContextType.CourseSection }
                };

                request.AssignmentGradeServices = new AssignmentGradeServicesClaimValueType
                {
                    Scope = new List<string>
                    {
                        Constants.LtiScopes.Ags.LineItem
                    },
                    LineItemUrl = gradebookColumn == null ? null : _linkGenerator.GetUriByRouteValues(Constants.ServiceEndpoints.Ags.LineItemService,
                        new { contextId = course.Id, lineItemId = gradebookColumn.Id }, httpRequest.Scheme, httpRequest.Host),
                    LineItemsUrl = _linkGenerator.GetUriByRouteValues(Constants.ServiceEndpoints.Ags.LineItemsService,
                        new { contextId = course.Id }, httpRequest.Scheme, httpRequest.Host)
                };

                request.NamesRoleService = new NamesRoleServiceClaimValueType
                {
                    ContextMembershipUrl = _linkGenerator.GetUriByRouteValues(Constants.ServiceEndpoints.Nrps.MembershipService,
                        new { contextId = course.Id }, httpRequest.Scheme, httpRequest.Host)
                };
            }

            // Collect custom properties
            if (!resourceLink.Tool.CustomProperties.TryConvertToDictionary(out var custom))
            {
                custom = new Dictionary<string, string>();
            }
            if (resourceLink.CustomProperties.TryConvertToDictionary(out var linkDictionary))
            {
                foreach (var property in linkDictionary)
                {
                    if (custom.ContainsKey(property.Key))
                    {
                        custom[property.Key] = property.Value;
                    }
                    else
                    {
                        custom.Add(property.Key, property.Value);
                    }
                }
            }

            // Prepare for custom property substitutions
            var substitutions = new CustomPropertySubstitutions
            {
                LtiUser = new LtiUser
                {
                    Username = person.UserName
                }
            };

            request.Custom = substitutions.ReplaceCustomPropertyValues(custom);

            return new List<Claim>(request.Claims);
        }
        private List<Claim> GetDeepLinkingRequestClaims(Tool tool, User person, Course course)
        {
            var httpRequest = _httpContextAccessor.HttpContext.Request;

            var request = new LtiDeepLinkingRequest
            {
                DeploymentId = tool.DeploymentId,
                FamilyName = person.LastName,
                GivenName = person.FirstName,
                LaunchPresentation = new LaunchPresentationClaimValueType
                {
                    DocumentTarget = DocumentTarget.Window,
                    Locale = CultureInfo.CurrentUICulture.Name
                },
                Lis = new LisClaimValueType
                {
                    PersonSourcedId = person.Id.ToString(),
                    CourseSectionSourcedId = course?.Id.ToString(),
                },
                Roles = ParsePersonRoles("ContextAdministrator, Administrator"),
                Lti11LegacyUserId = person.Id.ToString(),
                TargetLinkUri = tool.DeepLinkingLaunchUrl
            };

            // Add the context if the launch is from a course.
            if (course == null)
            {
                // Remove context roles
                request.Roles = request.Roles.Where(r => !r.ToString().StartsWith("Context")).ToArray();
            }
            else
            {
                request.Context = new ContextClaimValueType
                {
                    Id = course.Id.ToString(),
                    Title = course.Name,
                    Type = new[] { ContextType.CourseSection }
                };
            }

            // Add the deep linking settings
            request.DeepLinkingSettings = new DeepLinkingSettingsClaimValueType
            {
                AcceptPresentationDocumentTargets = new[] { DocumentTarget.Window },
                AcceptMultiple = true,
                AcceptTypes = new[] { Constants.ContentItemTypes.LtiLink },
                AutoCreate = true,
                DeepLinkReturnUrl = _linkGenerator.GetUriByPage(
                    "/DeepLinks",
                    handler: null,
                    values: new { courseId = course?.Id },
                    scheme: httpRequest.Scheme,
                    host: httpRequest.Host)
            };

            // Collect custom properties
            if (tool.CustomProperties.TryConvertToDictionary(out var custom))
            {
                // Prepare for custom property substitutions
                var substitutions = new CustomPropertySubstitutions
                {
                    LtiUser = new LtiUser
                    {
                        Username = person.UserName
                    }
                };

                request.Custom = substitutions.ReplaceCustomPropertyValues(custom);
            }

            return new List<Claim>(request.Claims);
        }
        public static Role[] ParsePersonRoles(string rolesString)
        {
            var roles = new List<Role>();
            foreach (var roleString in rolesString.Split(","))
            {
                if (Enum.TryParse<Role>(roleString, out var role))
                {
                    roles.Add(role);
                }
            }

            return roles.ToArray();
        }
    }
}