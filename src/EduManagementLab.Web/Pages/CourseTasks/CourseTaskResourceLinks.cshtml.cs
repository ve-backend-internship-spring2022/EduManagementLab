using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduManagementLab.Web.Pages.CourseLineItems
{
    [BindProperties]
    public class CourseTaskResourceLinksModel : PageModel
    {
        private readonly CourseLineItemService _courseTaskService;
        private readonly ResourceLinkService _resourceLinkService;
        public CourseTaskResourceLinksModel(CourseLineItemService courseLineItemService, ResourceLinkService resourceLinkService)
        {
            _courseTaskService = courseLineItemService;
            _resourceLinkService = resourceLinkService;
        }
        public List<SelectListItem> ResourceLinkSelectList { get; } = new List<SelectListItem>();
        public string SelctedResource { get; set; }
        public CourseLineItem CourseLineItem { get; set; }
        public IActionResult OnGet(Guid courseTaskId)
        {
            try
            {
                LoadData(courseTaskId);
            }
            catch (CourseLineItemNotFoundException ex)
            {
                NotFound(ex.Message);
            }
            return Page();
        }
        public void LoadData(Guid courseTaskId)
        {
            CourseLineItem = _courseTaskService.GetCourseLineItem(courseTaskId, false, true);
            foreach (var resource in _resourceLinkService.GetResourceLinks())
            {
                if (!CourseLineItem.IMSLTIResourceLinks.Any(r => r.Id == resource.Id))
                {
                    ResourceLinkSelectList.Add(new SelectListItem { Text = resource.Title, Value = resource.Id.ToString() });
                }
            }
        }
        public IActionResult OnPostAddResource(Guid courseTaskId)
        {
            try
            {
                _courseTaskService.AddResouceLinkToCourseTask(courseTaskId, Guid.Parse(SelctedResource));
            }
            catch (CourseLineItemNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            LoadData(courseTaskId);
            return Page();
        }
        public IActionResult OnPostRemoveResourceLinkFromCourseTask(Guid resourceId, Guid courseTaskId)
        {
            try
            {
                _courseTaskService.DeleteCourseTaskResoruceLink(courseTaskId, resourceId);
            }
            catch (CourseLineItemNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            LoadData(courseTaskId);
            return RedirectToPage("./CourseTaskResourceLinks", new { courseTaskId = courseTaskId });
        }
    }
}
