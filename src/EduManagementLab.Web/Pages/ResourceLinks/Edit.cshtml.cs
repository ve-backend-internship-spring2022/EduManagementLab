using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.ResourceLinks
{
    public class EditModel : PageModel
    {
        private readonly ResourceLinkService _resourceLinkService;
        private readonly ToolService _toolService;
        public EditModel(ResourceLinkService resourceLinkService, ToolService toolService)
        {
            _resourceLinkService = resourceLinkService;
            _toolService = toolService;
        }
        [BindProperty]
        public ResourceLinkInputModel ResourceLink { get; set; } = new ResourceLinkInputModel();
        public class ResourceLinkInputModel
        {
            public Guid Id { get; set; }
            public string? CustomProperties { get; set; }
            public string? Description { get; set; }
            public string Title { get; set; }
            [Required]
            public Guid selectedTool { get; set; }
            public List<SelectListItem> tools { get; set; } = new List<SelectListItem>();
        }
        public void OnGet(Guid resourceId)
        {
            var targetResource = _resourceLinkService.GetResourceLink(resourceId);
            ResourceLink.Id = targetResource.Id;
            ResourceLink.CustomProperties = targetResource.CustomProperties;
            ResourceLink.Description = targetResource.Description;
            ResourceLink.Title = targetResource.Title;
            ResourceLink.selectedTool = targetResource.Tool.Id;

            var toolList = _toolService.GetTools();
            foreach (var tool in toolList)
            {
                ResourceLink.tools.Add(new SelectListItem { Text = tool.Name, Value = tool.Id.ToString() });
            }
        }
        public IActionResult OnPost(Guid id)
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
                        _resourceLinkService.UpdateResourceLink(id, newResource);
                    }
                    catch (IMSLTIResourceLinkNotFoundException)
                    {
                        return NotFound();
                    }

                    return RedirectToPage("./Index");
                }
            }
            return Page();
        }
    }
}
