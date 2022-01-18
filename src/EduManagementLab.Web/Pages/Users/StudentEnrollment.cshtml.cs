using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Users
{
    public class StudentEnrollmentModel : PageModel
    {
        private readonly CourseService _courseService;

        public StudentEnrollmentModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        public IList<Course.Membership> UserCourses { get; set; }
        //public async Task<IActionResult> OnGetAsync(Guid id)
        //{
        //    try
        //    {
        //        UserCourses = _courseService.GetCourseMembership(id).ToList();
        //        return Page();
        //    }
        //    catch (CourseNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}
    }
}
