using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Tools
{
    public class DetailModel : PageModel
    {
        private readonly ToolService _ToolService;
        private readonly IConfiguration _configuration;
        private readonly OAuthClientService _OAuthClientService;
        public DetailModel(ToolService IToolService, IConfiguration configuration, OAuthClientService OAuthClientService)
        {
            _ToolService = IToolService;
            _configuration = configuration;
            _OAuthClientService = OAuthClientService;
        }
        [BindProperty]
        public ToolModel Tool { get; set; } = new ToolModel();
        [BindProperty]
        public IdentityServer4 IdentityServer { get; set; } = new IdentityServer4();
        public class ToolModel
        {
            [Display(Name = "Client ID")]
            public string ClientId { get; set; }
            [Display(Name = "Public Key", Description = "Public key to validate messages signed by the tool.")]
            public string PublicKey { get; set; }
            [Display(Name = "Custom Properties", Description = "Custom properties to include in all launches of this tool deployment.")]
            public string? CustomProperties { get; set; }
            [Display(Name = "Deep Linking Launch URL", Description = "The URL to launch the tool's deep linking experience.")]
            public string DeepLinkingLaunchUrl { get; set; }
            [Display(Name = "Deployment ID", Description = "Unique id assigned to this tool deployment.")]
            public string DeploymentId { get; set; }
            [Display(Name = "Launch URL", Description = "The URL to launch the tool.")]
            public string LaunchUrl { get; set; }
            [Display(Name = "Login URL", Description = "The URL to initiate OpenID Connect authorization.")]
            public string LoginUrl { get; set; }
            [Display(Name = "Display Name")]
            public string Name { get; set; }
        }
        public class IdentityServer4
        {
            /// <summary>
            /// Identity Server issuer uri
            /// </summary>
            [Display(Name = "Issuer")]
            public string? Issuer { get; set; }

            /// <summary>
            /// Identity Server authorize endpoint url
            /// </summary>
            [Display(Name = "Authorize URL")]
            public string? AuthorizeUrl { get; set; }

            /// <summary>
            /// Identity Server JWK Set endpoint url
            /// </summary>
            [Display(Name = "JWK Set URL")]
            public string? JwkSetUrl { get; set; }

            /// <summary>
            /// Identity Server access token endpoint uri
            /// </summary>
            [Display(Name = "Access Token URL")]
            public string? TokenUrl { get; set; }
        }

        public void OnGet(Guid toolId)
        {
            loadToolInfo(toolId);
        }
        public void loadToolInfo(Guid toolId)
        {
            var targetTool = _ToolService.GetTool(toolId);
            var targetClient = _OAuthClientService.FindClientByClientId(targetTool.IdentityServerClientId);

            Tool.Name = targetTool.Name;
            Tool.LaunchUrl = targetTool.LaunchUrl;
            Tool.LoginUrl = targetTool.LoginUrl;
            Tool.CustomProperties = targetTool.CustomProperties;
            Tool.DeploymentId = targetTool.DeploymentId;
            Tool.DeepLinkingLaunchUrl = targetTool.DeepLinkingLaunchUrl;
            Tool.ClientId = targetTool.IdentityServerClientId;
            Tool.PublicKey = targetClient.ClientSecrets.FirstOrDefault(c => c.Client == targetClient).Value;

            var identityServer = _configuration.GetSection("IdentityServer");

            IdentityServer.Issuer = identityServer.GetSection("Issuer").Value;
            IdentityServer.AuthorizeUrl = identityServer.GetSection("AuthorizeUrl").Value;
            IdentityServer.JwkSetUrl = identityServer.GetSection("JwkSetUrl").Value;
            IdentityServer.TokenUrl = identityServer.GetSection("TokenUrl").Value;
        }
    }
}
