using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer4.Interfaces;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace EduManagementLab.Web.Pages.Tools
{
    public class EditModel : PageModel
    {
        private readonly ToolService _ToolService;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationDbContext _identityConfig;
        public EditModel(ToolService IToolService, IConfiguration configuration, IConfigurationDbContext configurationDbContext)
        {
            _ToolService = IToolService;
            _configuration = configuration;
            _identityConfig = configurationDbContext;
        }
        [BindProperty]
        public ToolModel tool { get; set; } = new ToolModel();
        [BindProperty]
        public IdentityServer4 IdentityServer { get; set; } = new IdentityServer4();
        public class ToolModel
        {
            public Guid Id { get; set; }
            [Required]
            [Display(Name = "Client ID")]
            public string ClientId { get; set; }
            [Required]
            [Display(Name = "Public Key", Description = "Public key to validate messages signed by the tool.")]
            public string PublicKey { get; set; }
            [Display(Name = "Custom Properties", Description = "Custom properties to include in all launches of this tool deployment.")]
            public string? CustomProperties { get; set; }

            [Display(Name = "Deep Linking Launch URL", Description = "The URL to launch the tool's deep linking experience.")]
            public string DeepLinkingLaunchUrl { get; set; }

            [Display(Name = "Deployment ID", Description = "Unique id assigned to this tool deployment.")]
            public string DeploymentId { get; set; }

            [Required]
            [Display(Name = "Launch URL", Description = "The URL to launch the tool.")]
            public string LaunchUrl { get; set; }

            [Required]
            [Display(Name = "Login URL", Description = "The URL to initiate OpenID Connect authorization.")]
            public string LoginUrl { get; set; }

            [Required]
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
            loadStaticToolInfo(toolId);
        }
        public void loadStaticToolInfo(Guid toolId)
        {
            var targetTool = _ToolService.GetTool(toolId);
            var targetClient = _identityConfig.Clients.Include(s => s.ClientSecrets).FirstOrDefault(c => c.ClientId == targetTool.IdentityServerClientId);

            tool.Id = targetTool.Id;
            tool.Name = targetTool.Name;
            tool.LaunchUrl = targetTool.LaunchUrl;
            tool.LoginUrl = targetTool.LoginUrl;
            tool.CustomProperties = targetTool.CustomProperties;
            tool.DeploymentId = targetTool.DeploymentId;
            tool.DeepLinkingLaunchUrl = targetTool.DeepLinkingLaunchUrl;
            tool.ClientId = targetTool.IdentityServerClientId;
            tool.PublicKey = targetClient.ClientSecrets.FirstOrDefault(c => c.Client == targetClient).Value;

            var identityServer = _configuration.GetSection("IdentityServer");

            IdentityServer.Issuer = identityServer.GetSection("Issuer").Value;
            IdentityServer.AuthorizeUrl = identityServer.GetSection("AuthorizeUrl").Value;
            IdentityServer.JwkSetUrl = identityServer.GetSection("JwkSetUrl").Value;
            IdentityServer.TokenUrl = identityServer.GetSection("TokenUrl").Value;
        }
        public IActionResult OnPost(Guid toolId)
        {
            if (ModelState.IsValid)
            {
                Tool updateTool = new Tool();
                {
                    updateTool.Name = tool.Name;
                    updateTool.LaunchUrl = tool.LaunchUrl;
                    updateTool.LoginUrl = tool.LoginUrl;
                    updateTool.CustomProperties = tool.CustomProperties;
                    updateTool.DeepLinkingLaunchUrl = tool.DeepLinkingLaunchUrl;
                    updateTool.DeploymentId = tool.DeploymentId;
                    updateTool.IdentityServerClientId = tool.ClientId;
                }
                _ToolService.UpdateTool(toolId, updateTool);
            }
            return Page();
        }

    }
}
