using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer4.Extensions;

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
        [BindProperty]
        public CourseLineItem CourseLineItem { get; set; } = new CourseLineItem();
        [BindProperty]
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid lineItemId, Guid courseId)
        {
            UserId = Guid.Parse(User?.GetSubjectId());
            CourseId = courseId;
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
