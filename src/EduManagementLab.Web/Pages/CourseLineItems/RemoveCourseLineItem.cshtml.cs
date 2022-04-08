using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.CourseLineItems
{
    public class RemoveCourseLineItemModel : PageModel
    {
        private readonly CourseLineItemService _courseLineItemService;
        public RemoveCourseLineItemModel(CourseLineItemService courseLineItemService)
        {
            _courseLineItemService = courseLineItemService;
        }
        [BindProperty]
        public CourseLineItemDto courseLineItemDto { get; set; } = new CourseLineItemDto();
        [BindProperty]
        public Guid courseId { get; set; }
        public class CourseLineItemDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Active { get; set; }
        }
        public void OnGet(Guid lineItemId, Guid courseId)
        {
            this.courseId = courseId;
            var courseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId);
            courseLineItemDto.Id = courseLineItem.Id;
            courseLineItemDto.Name = courseLineItem.Name;
            courseLineItemDto.Description = courseLineItem.Description;
        }
        public IActionResult OnPost(Guid courseId, Guid lineItemId)
        {
            _courseLineItemService.DeleteCourseLineItem(lineItemId);
            return RedirectToPage("../Courses/Details", new { courseId = courseId });
        }
    }
}
