using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Courses
{
    public class RemoveMembershipModel : PageModel
    {
        private readonly CourseService _courseService;
        private readonly UserService _userService;

        public RemoveMembershipModel(CourseService courseService, UserService userService)
        {
            _courseService = courseService;
            _userService = userService; 
        }

        public Course Course { get; set; }
        public User User { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid userId)
        {
            try
            {
                User = _userService.GetUser(userId);

                return Page();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid courseId, Guid userId)
        {
            try
            {
                _courseService.RemoveCourseMembership(courseId, userId);

                return RedirectToPage("./Details", new { courseId = courseId });
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
