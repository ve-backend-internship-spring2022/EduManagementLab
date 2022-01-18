using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Courses
{
    public class DeleteModel : PageModel
    {
        private readonly CourseService _courseService;

        public DeleteModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                Course = _courseService.GetCourse(id);
                return Page();
            }
            catch (CourseNotFoundException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            try
            {
                _courseService.DeleteCourse(id);
                return RedirectToPage("./Index");
            }
            catch (CourseNotFoundException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return NotFound();
            }
        }
    }
}
