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
        private readonly CourseLineItemService _courseLineItemService;
        public AssignmentDetailsModel(CourseLineItemService courseLineItemService)
        {
            _courseLineItemService = courseLineItemService;
        }
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
                CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true, true);
                return Page();
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
