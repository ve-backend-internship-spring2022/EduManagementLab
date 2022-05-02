using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.CourseLineItems
{
    public class EditCourseLineItemModel : PageModel
    {
        private readonly CourseLineItemService _courseLineItemService;
        private readonly ResourceLinkService _resourceLinkService;
        public EditCourseLineItemModel(CourseLineItemService courseLineItemService, ResourceLinkService resourceLinkService)
        {
            _courseLineItemService = courseLineItemService;
            _resourceLinkService = resourceLinkService;
        }
        public List<SelectListItem> Resources { get; set; } = new List<SelectListItem>();
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        [Required]
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string? Description { get; set; }

        public void OnGet(Guid id, Guid courseId)
        {
            var courseLineItem = _courseLineItemService.GetCourseLineItem(id);
            CourseId = courseId;
            Id = courseLineItem.Id;
            Name = courseLineItem.Name;
            Description = courseLineItem.Description;

            var resourceLinks = _resourceLinkService.GetResourceLinks();
            foreach (var item in resourceLinks)
            {
                Resources.Add(new SelectListItem { Text = item.Title, Value = item.Id.ToString() });
            }
        }
        public IActionResult OnPost(Guid id, Guid courseId)
        {
            if (ModelState.IsValid)
            {
                _courseLineItemService.UpdateCourseLineItemInfo(id, Name, Description);
            }
            return RedirectToPage("./Details", new { lineItemId = id, courseId = courseId });
        }
    }
}
