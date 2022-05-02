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
        private readonly CourseTaskService _courseTaskService;
        private readonly ResourceLinkService _resourceLinkService;
        public CourseTaskResourceLinksModel(CourseTaskService courseLineItemService, ResourceLinkService resourceLinkService)
        {
            _courseTaskService = courseLineItemService;
            _resourceLinkService = resourceLinkService;
        }
        public List<SelectListItem> ResourceLinkSelectList { get; } = new List<SelectListItem>();
        public string SelctedResource { get; set; }
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

            //foreach (var resourceLink in _resourceLinkService.GetResourceLinks()
            //    .Where(s => !courseTask.IMSLTIResourceLinks.Any(x => x.Id == s.Id) &&
            //    courseTaskList.Any(c => !c.IMSLTIResourceLinks.Any(r => r.Id == s.Id))))
            //{

            //    ResourceLinkSelectList.Add(new SelectListItem { Text = resourceLink.Title, Value = resourceLink.Id.ToString() });
            //}
        }
        public IActionResult OnPostAddResourceLinkToCourseTask(Guid courseTaskId)
        {
            try
            {
                _courseTaskService.AddResouceLinkToCourseTask(courseTaskId, Guid.Parse(SelctedResource));
            }
            catch (CourseTaskNotFoundException ex)
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
            catch (CourseTaskNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            LoadData(courseTaskId);
            return RedirectToPage("./CourseTaskResourceLinks", new { courseTaskId = courseTaskId });
        }
    }
}
