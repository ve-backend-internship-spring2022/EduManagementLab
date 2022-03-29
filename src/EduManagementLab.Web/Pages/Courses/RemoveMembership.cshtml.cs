using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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
        [BindProperty]
        public UserDto User { get; set; } = new UserDto();
        [BindProperty]
        public bool DeleteResult { get; set; }
        public class UserDto
        {
            public Guid Id { get; set; }
            public string Displayname { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(Guid userId)
        {
            try
            {
                var user = _userService.GetUser(userId);
                User.Id = user.Id;
                User.Displayname = user.Displayname;
                User.FirstName = user.FirstName;
                User.LastName = user.LastName;

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
                if (DeleteResult == true)
                {
                    _courseService.RemoveCourseMembership(courseId, userId, true);
                }
                else
                {
                    _courseService.RemoveCourseMembership(courseId, userId);
                }

                return RedirectToPage("./Details", new { courseId = courseId });
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
