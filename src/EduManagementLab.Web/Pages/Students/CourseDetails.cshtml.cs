using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Students
{
    public class CourseDetailsModel : PageModel
    {
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        private readonly CourseTaskService _courseTaskService;
        public CourseDetailsModel(CourseService courseService, UserService userService, CourseTaskService courseTaskService)
        {
            _courseService = courseService;
            _userService = userService;
            _courseTaskService = courseTaskService;
        }
        public Course Course { get; set; }
        public SelectList courseTaskListItems { get; set; }
        [BindProperty]
        public courseTaskInputModel courseTaskInput { get; set; }
        public class courseTaskInputModel
        {
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; }
            [Required]
            public bool Active { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid courseId)
        {
            try
            {
                PopulateProperties(courseId);
                return Page();
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
        private void PopulateProperties(Guid courseId)
        {
            Course = _courseService.GetCourse(courseId, true);

            courseTaskListItems = new SelectList(_courseTaskService.GetCourseTasks()
               .Where(s => !Course.CourseTasks.Any(x => x.Name == s.Name)), "Name", "Description");
        }
    }
}
