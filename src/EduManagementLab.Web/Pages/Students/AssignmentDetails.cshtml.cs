using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Students
{
    public class AssignmentDetailsModel : PageModel
    {
        private readonly CourseTaskService _courseLineItemService;
        public AssignmentDetailsModel(CourseTaskService courseLineItemService)
        {
            _courseLineItemService = courseLineItemService;
        }
        public CourseTask CourseLineItem { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid lineItemId)
        {
            try
            {
                CourseLineItem = _courseLineItemService.GetCourseTask(lineItemId, true);
                return Page();
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
