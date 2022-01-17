using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static EduManagementLab.Core.Entities.Course;

namespace EduManagementLab.Web.Pages.Courses
{
    public class EditModel : PageModel
    {
        private readonly CourseService _courseService;

        public EditModel(CourseService courseService)
        {
            _courseService = courseService;
        }
        [BindProperty]
        [Required]
        public Guid CourserId { get; set; }
        [BindProperty]
        [Required]
        public string Code { get; set; }
        [BindProperty]
        [Required]
        public string Name { get; set; }
        [BindProperty]
        public string? Description { get; set; }
        [BindProperty]
        [Required]
        public DateTime StartDate { get; set; }
        [BindProperty]
        [Required]
        public DateTime EndDate { get; set; }
        [BindProperty]
        [Required]
        public List<Membership>? Memberships { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var course = _courseService.GetCourse(id);
                CourserId = course.Id;
                Code = course.Code;
                Name = course.Name;
                Description = course.Description;
                StartDate = course.StartDate;
                EndDate = course.EndDate;   

                return Page();
            }
            catch (CourseNotFoundException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _courseService.UpdateCourseInfo(UserId, Displayname, FirstName, LastName);
                return RedirectToPage("./Index");
            }
            catch (UserNotFoundException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return Page();
            }
        }
    }
}
