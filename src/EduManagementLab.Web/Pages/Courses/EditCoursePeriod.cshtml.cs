using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static EduManagementLab.Core.Entities.Course;

namespace EduManagementLab.Web.Pages.Courses
{
    public class EditCoursePeriodModel : PageModel
    {
        private readonly CourseService _courseService;

        public EditCoursePeriodModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        [Required]
        public Guid CourseId { get; set; }
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
                CourseId = course.Id;
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
                _courseService.UpdateCoursePeriod(CourseId, StartDate, EndDate);
                return RedirectToPage("./Index");
            }
            catch (CourseNotFoundException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return Page();
            }
        }
    }
}
