using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.CourseLineItems
{
    [BindProperties]
    public class CourseTaskResourceLinksModel : PageModel
    {
        private readonly CourseTaskService _courseTaskService;
        private readonly ResourceLinkService _resourceLinkService;
        private readonly ToolService _toolService;
        public CourseTaskResourceLinksModel(CourseTaskService courseLineItemService, ResourceLinkService resourceLinkService, ToolService toolService)
        {
            _courseTaskService = courseLineItemService;
            _resourceLinkService = resourceLinkService;
            _toolService = toolService;
        }
        public List<SelectListItem> ResourceLinkSelectList { get; } = new List<SelectListItem>();
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
        public CourseTask courseTask { get; set; }
        public IActionResult OnGet(Guid courseTaskId)
        {
            try
            {
                LoadData(courseTaskId);
            }
            catch (CourseTaskNotFoundException ex)
            {
                NotFound(ex.Message);
            }
            return Page();
        }
        public void LoadData(Guid courseTaskId)
        {
            courseTask = _courseTaskService.GetCourseTask(courseTaskId, false, true);

            foreach (var resourceLink in _resourceLinkService.GetResourceLinks())
            {

                ResourceLinkSelectList.Add(new SelectListItem { Text = resourceLink.Title, Value = resourceLink.Id.ToString() });
            }

            foreach (var tool in _toolService.GetTools())
            {
                tools.Add(new SelectListItem { Text = tool.Name, Value = tool.Id.ToString() });
            }
        }
        public IActionResult OnPost(Guid courseTaskId)
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
                    _courseTaskService.AddResouceLinkToCourseTask(courseTaskId, newResource);
                }
                catch (IMSLTIResourceLinkAlreadyExistException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            LoadData(courseTaskId);
            return RedirectToPage("./CourseTaskResourceLinks", new { courseTaskId = courseTaskId });
        }
        public IActionResult OnPostRemoveResourceLink(Guid resourceId, Guid courseTaskId)
        {
            try
            {
                _resourceLinkService.DeleteResourceLink(resourceId);
            }
            catch (CourseTaskNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            LoadData(courseTaskId);
            return Page();
        }
    }
}
