using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.CourseLineItems
{
    public class EditCourseLineItemModel : PageModel
    {
        private readonly CourseLineItemService _courseLineItemService;
        public EditCourseLineItemModel(CourseLineItemService courseLineItemService)
        {
            _courseLineItemService = courseLineItemService;
        }
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
