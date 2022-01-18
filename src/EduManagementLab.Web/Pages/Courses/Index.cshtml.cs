using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly CourseService _courseService;

        public IndexModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        public IList<Course> Courses { get; set; }

        public async Task OnGetAsync()
        {
            Courses = _courseService.GetCourses().ToList();
        }
    }
}
