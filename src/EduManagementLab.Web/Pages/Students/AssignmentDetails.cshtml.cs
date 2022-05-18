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
        private readonly CourseTaskService _courseTaskService;
        public AssignmentDetailsModel(CourseTaskService courseTaskService)
        {
            _courseTaskService = courseTaskService;
        }
        public CourseTask CourseTask { get; set; } = new CourseTask();
        [BindProperty]
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid courseTaskId, Guid courseId)
        {
            UserId = Guid.Parse(User?.GetSubjectId());
            CourseId = courseId;
            try
            {
                CourseTask = _courseTaskService.GetCourseTask(courseTaskId, true);
                return Page();
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
