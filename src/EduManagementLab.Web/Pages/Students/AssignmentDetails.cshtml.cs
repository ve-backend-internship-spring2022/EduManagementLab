using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Students
{
    public class AssignmentDetailsModel : PageModel
    {
        private readonly CourseLineItemService _courseLineItemService;
        public AssignmentDetailsModel(CourseLineItemService courseLineItemService)
        {
            _courseLineItemService = courseLineItemService;
        }
        public CourseLineItem CourseLineItem { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid lineItemId)
        {
            try
            {
                CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true);
                return Page();
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
