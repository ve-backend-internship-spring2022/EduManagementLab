using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Users
{
    public class RemoveCourseModel : PageModel
    {
        private readonly CourseService _courseService;
        private readonly UserService _userService;

        public RemoveCourseModel(CourseService courseService, UserService userService)
        {
            _courseService = courseService;
            _userService = userService;
        }

        public Course Course { get; set; }
        public User User { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid courseId)
        {
            try
            {
                Course = _courseService.GetCourse(courseId);

                return Page();
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid courseId, Guid userId)
        {
            try
            {
                _courseService.RemoveCourseMembership(courseId, userId);

                return RedirectToPage("./Details", new { userId = userId });
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
