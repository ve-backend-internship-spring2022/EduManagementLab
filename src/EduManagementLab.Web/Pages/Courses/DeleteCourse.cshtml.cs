using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Courses
{
    public class DeleteCourseModel : PageModel
    {
        private readonly CourseService _courseService;

        public DeleteCourseModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        public Course Course { get; set; }

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

        public async Task<IActionResult> OnPostAsync(Guid courseId)
        {
            try
            {
                _courseService.DeleteCourse(courseId);

                return RedirectToPage("./Index");
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
