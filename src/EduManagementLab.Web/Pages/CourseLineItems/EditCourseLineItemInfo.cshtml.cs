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
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public void OnGet(Guid id)
        {
            var courseLineItem = _courseLineItemService.GetCourseLineItem(id);
            Id = courseLineItem.Id;
            Name = courseLineItem.Name;
            Description = courseLineItem.Description;
        }
        public IActionResult OnPost(Guid id)
        {
            if (ModelState.IsValid)
            {
                _courseLineItemService.UpdateCourseLineItemInfo(id, Name, Description);
            }
            return Page();
        }
    }
}
