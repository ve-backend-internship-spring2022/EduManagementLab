using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public Course Course { get; set; }
        public SelectList UserListItems { get; set; }

        [BindProperty]
        public DateTime EnrolledDate { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        [BindProperties]
        public class InputModel
        {
            public Guid Id { get; set; }
            [Required(ErrorMessage = "Displayname Required!")]
            public string DisplayName { get; set; }
            [Required(ErrorMessage = "Firstname Required!")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "Lastname Required!")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "Email Required!")]
            [DataType(DataType.EmailAddress, ErrorMessage = "Please type a valid email address")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid courseId)
        {
            try
            {
                Course = _courseService.GetCourse(courseId, true);

                UserListItems = new SelectList(_userService.GetUsers()
                    .Where(s => !Course.Memperships.Any(x => x.User.Email == s.Email)), "Id", "Email");

                return Page();
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostExistingUserAsync(Guid courseId, Guid Id)
        {
            try
            {
                var existingUser = _userService.GetUser(Id);
                _courseService.CreateCourseMembership(courseId, existingUser.Id, DateTime.Now);

                return RedirectToPage("./Details", new { courseId = courseId });
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostNewUserAsync(Guid courseId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newUser = _userService.CreateUser(Input.DisplayName, Input.FirstName, Input.LastName, Input.Email);
                    _courseService.CreateCourseMembership(courseId, newUser.Id, DateTime.Now);

                    return RedirectToPage("./Details", new { courseId = courseId });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Input.Email", "User already exist");
                    ViewData["Input.Email"] = !string.IsNullOrEmpty(Input.Email) ? true : false;
                    OnGetAsync(courseId);
                    return Page();
                }
            }
            else
            {
                OnGetAsync(courseId);
                return Page();
            }
            return RedirectToAction("OnGetAsync");
        }
    }
}
