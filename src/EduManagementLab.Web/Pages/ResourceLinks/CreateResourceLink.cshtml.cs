using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.ResourceLinks
{
    public class CreateResourceLinkModel : PageModel
    {
        private readonly ResourceLinkService _resourceLinkService;
        private readonly ToolService _toolService;
        public CreateResourceLinkModel(ResourceLinkService resourceLinkService, ToolService toolService)
        {
            _resourceLinkService = resourceLinkService;
            _toolService = toolService;
        }
        [BindProperty]
        public ResourceLinkInputModel ResourceLink { get; set; } = new ResourceLinkInputModel();
        public class ResourceLinkInputModel
        {
            public string? CustomProperties { get; set; }
            public string? Description { get; set; }
            [Required]
            public string Title { get; set; }
            [Required]
            public Guid selectedTool { get; set; }
        }
        public List<SelectListItem> tools { get; set; } = new List<SelectListItem>();
        public void OnGet()
        {
            LoadToolsAndCourses();
        }
        public void LoadToolsAndCourses()
        {
            var toolList = _toolService.GetTools();
            foreach (var tool in toolList)
            {
                tools.Add(new SelectListItem { Text = tool.Name, Value = tool.Id.ToString() });
            }
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var targetTool = _toolService.GetTool(ResourceLink.selectedTool);

                if (targetTool != null)
                {
                    IMSLTIResourceLink newResource = new IMSLTIResourceLink()
                    {
                        Description = ResourceLink.Description,
                        Title = ResourceLink.Title,
                        Tool = targetTool,
                        CustomProperties = ResourceLink.CustomProperties,
                    };

                    try
                    {
                        _resourceLinkService.CreateResourceLink(newResource);
                        return RedirectToPage("./Index");
                    }
                    catch (IMSLTIResourceLinkToolDublicatedException ex)
                    {
                        ModelState.AddModelError(String.Empty, ex.Message);
                    }
                }
            }
            LoadToolsAndCourses();
            return Page();
        }
    }
}
