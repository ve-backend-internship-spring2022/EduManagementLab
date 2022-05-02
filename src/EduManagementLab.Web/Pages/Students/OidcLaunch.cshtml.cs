using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer;
using EduManagementLab.Core.Validation;
using EduManagementLab.IdentityServer4.Interfaces;
using IdentityModel.Client;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace EduManagementLab.Web.Pages.Students
{
    public class OidcLaunchModel : PageModel
    {
        private readonly ILogger<OidcLaunchModel> _logger;
        private readonly ResourceLinkService _resourceLinkService;
        private readonly ToolService _toolService;
        private readonly IConfigurationDbContext _identityConfig;
        private readonly IConfiguration _configuration;
        public OidcLaunchModel(
            ILogger<OidcLaunchModel> logger,
            ResourceLinkService resourceLinkService,
            ToolService toolService,
            IConfigurationDbContext configurationDbContext,
            IConfiguration configuration)
        {
            _logger = logger;
            _resourceLinkService = resourceLinkService;
            _toolService = toolService;
            _identityConfig = configurationDbContext;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync(Guid id, string messageType, string courseId, string personId)
        {
            Tool tool = new Tool();
            IMSLTIResourceLink resourceLink = new IMSLTIResourceLink();
            if (messageType == Constants.Lti.LtiResourceLinkRequestMessageType)
            {
                resourceLink = _resourceLinkService.GetResourceLink(id);
                if (resourceLink == null)
                {
                    _logger.LogError("Resource link not found.");
                    return BadRequest();
                }

                tool = resourceLink.Tool;
            }
            else
            {
                tool = _toolService.GetTool(id);
            }

            if (tool == null)
            {
                _logger.LogError("Tool not found.");
                return BadRequest();
            }

            //var client = Config.Clients.FirstOrDefault(t => t.ClientId == tool.IdentityServerClientId);
            var client = _identityConfig.Clients.FirstOrDefault(c => c.ClientId == tool.IdentityServerClientId);
            if (client == null)
            {
                _logger.LogError("Client not found");
                return BadRequest();
            }

            // The issuer identifier for the platform
            string iss = _configuration.GetSection("IdentityServer").GetSection("Issuer").Value,

            // The platform identifier for the user to login
            login_hint = personId,
            // The endpoint to be executed at the end of the OIDC authentication flow
            target_link_uri = tool.LaunchUrl,

            // The identifier of the LtiResourceLink message (or the deep link message, etc)
            lti_message_hint = JsonConvert.SerializeObject(new { id, messageType, courseId });

            Parameters values = new Parameters();
            values.Add("iss", iss);
            values.Add("login_hint", login_hint);
            values.Add("target_link_uri", target_link_uri);
            values.Add("lti_message_hint", lti_message_hint);

            var url = new RequestUrl(tool.LoginUrl).Create(values);
            _logger.LogInformation($"Launching {tool.Name} using GET {url}");
            return Redirect(url);
        }
    }
}
