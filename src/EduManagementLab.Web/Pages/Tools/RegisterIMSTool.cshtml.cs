using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer;
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Tools
{
    public class RegisterIMSToolModel : PageModel
    {
        private readonly ToolService _ToolService;
        private readonly IConfiguration _configuration;
        private readonly OAuthClientService _oauthClientService;
        public RegisterIMSToolModel(ToolService IToolService,
            OAuthClientService oauthClientService,
            IConfiguration configuration)
        {
            _ToolService = IToolService;
            _configuration = configuration;
            _oauthClientService = oauthClientService;
        }
        [BindProperty]
        public ToolModel tool { get; set; } = new ToolModel();
        public class ToolModel
        {
            public int IdentityServerClientId { get; set; }
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
            public IdentityServer4 IdentityServer { get; set; } = new IdentityServer4();
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

        public void OnGet()
        {
            loadStaticToolInfo();
        }
        public void loadStaticToolInfo()
        {
            tool.ClientId = CryptoRandom.CreateUniqueId(8);
            tool.DeploymentId = CryptoRandom.CreateUniqueId(8);

            var identityServerCredential = _configuration.GetSection("IdentityServer");

            tool.IdentityServer.Issuer = identityServerCredential.GetSection("Issuer").Value;
            tool.IdentityServer.AuthorizeUrl = identityServerCredential.GetSection("AuthorizeUrl").Value;
            tool.IdentityServer.JwkSetUrl = identityServerCredential.GetSection("JwkSetUrl").Value;
            tool.IdentityServer.TokenUrl = identityServerCredential.GetSection("TokenUrl").Value;
        }
        public IActionResult OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (!_oauthClientService.ValidateCredentials(tool.ClientId, tool.PublicKey))
                {
                    _oauthClientService.CreateOAuthClient(tool.ClientId, tool.Name, tool.LaunchUrl, tool.PublicKey);
                    var newTool = new Tool
                    {
                        CustomProperties = tool.CustomProperties,
                        DeepLinkingLaunchUrl = tool.DeepLinkingLaunchUrl,
                        DeploymentId = tool.DeploymentId,
                        IdentityServerClientId = tool.ClientId,
                        Name = tool.Name,
                        LaunchUrl = tool.LaunchUrl,
                        LoginUrl = tool.LoginUrl,
                    };
                    _ToolService.CreateTool(newTool);
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Client already exist!");
                    return Page();
                }
            }
            loadStaticToolInfo();
            return Page();
        }
    }
}
