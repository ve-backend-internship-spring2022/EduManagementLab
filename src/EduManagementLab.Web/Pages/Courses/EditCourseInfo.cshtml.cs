using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static EduManagementLab.Core.Entities.Course;

namespace EduManagementLab.Web.Pages.Courses
{
    public class EditCourseInfoModel : PageModel
    {
        private readonly CourseService _courseService;

        public EditCourseInfoModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        [Required]
        public Guid CourseId { get; set; }
        [BindProperty]
        [Required]
        public string Code { get; set; }
        [BindProperty]
        [Required]
        public string Name { get; set; }
        [BindProperty]
        public string? Description { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var course = _courseService.GetCourse(id);
                CourseId = course.Id;
                Code = course.Code;
                Name = course.Name;
                Description = course.Description;

                return Page();
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
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
                _courseService.UpdateCourseInfo(CourseId, Code, Name, Description);
                return RedirectToPage("./Details", new { CourseId = CourseId });
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
