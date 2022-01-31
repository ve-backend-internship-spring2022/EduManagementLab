using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Courses
{
    public class DetailsModel : PageModel
    {
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        public DetailsModel(CourseService courseService, UserService userService)
        {
            _courseService = courseService;
            _userService = userService;
        }
        public User User { get; set; }

        public Course Course { get; set; }
        [BindProperty]
        public DateTime EnrolledDate { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        [BindProperties]
        public class InputModel
        {
            [Required]
            public string DisplayName { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid courseId)
        {
            try
            {
                Course = _courseService.GetCourse(courseId, true);
                return Page();
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> OnPostAsync(Guid courseId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var user = _userService.CreateUser(Input.DisplayName, Input.FirstName, Input.LastName, Input.Email);

                _courseService.CreateCourseMembership(courseId, user.Id, DateTime.Now);
                //OnGetAsync(courseId);
                return RedirectToPage("./Details", new { courseId = courseId });
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
