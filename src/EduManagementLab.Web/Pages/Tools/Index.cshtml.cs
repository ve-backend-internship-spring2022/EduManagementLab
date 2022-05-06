using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Tools
{
    public class IndexModel : PageModel
    {
        private readonly ToolService _ToolService;
        private readonly OAuthClientService _OAuthClientService;
        public IndexModel(ToolService iMSToolService, OAuthClientService OAuthClientService)
        {
            _ToolService = iMSToolService;
            _OAuthClientService = OAuthClientService;
        }
        public List<Tool> Tools { get; set; } = new List<Tool>();
        public void OnGet()
        {
            Tools = _ToolService.GetTools().ToList();
        }
        public IActionResult OnPostDeleteTool(Guid toolId)
        {
            var targetTool = _ToolService.GetTool(toolId);

            var targetClient = _OAuthClientService.GetOAuthClientById(targetTool.IdentityServerClientId);
            if (targetClient != null)
            {
                _OAuthClientService.DeleteOAuthClientById(targetClient.Id);
            }
            _ToolService.DeleteTool(toolId);
            Tools = _ToolService.GetTools().ToList();
            return RedirectToPage("./Index");
        }
    }
}
