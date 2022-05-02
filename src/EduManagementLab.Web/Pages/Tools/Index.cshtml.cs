using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer4.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Tools
{
    public class IndexModel : PageModel
    {
        private readonly ToolService _ToolService;
        private readonly IConfigurationDbContext _identityConfig;
        public IndexModel(ToolService iMSToolService, IConfigurationDbContext configurationDbContext)
        {
            _ToolService = iMSToolService;
            _identityConfig = configurationDbContext;
        }
        public List<Tool> Tools { get; set; } = new List<Tool>();
        public void OnGet()
        {
            Tools = _ToolService.GetTools().ToList();
        }
        public IActionResult OnPostDeleteTool(Guid toolId)
        {
            var targetTool = _ToolService.GetTool(toolId);

            var targetClient = _identityConfig.Clients.FirstOrDefault(c => c.ClientId == targetTool.IdentityServerClientId);
            if (targetClient != null)
            {
                _identityConfig.Clients.Remove(targetClient);
                _identityConfig.SaveChanges();
            }
            _ToolService.DeleteTool(toolId);
            Tools = _ToolService.GetTools().ToList();
            return RedirectToPage("./Index");
        }
    }
}
