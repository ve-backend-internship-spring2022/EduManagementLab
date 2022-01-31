#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EduManagementLab.Core.Services;
using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;

namespace EduManagementLab.Web.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly UserService _userService; 
        private readonly CourseService _courseService;

        public DetailsModel(UserService userService, CourseService courseService)
        {
            _userService = userService;
            _courseService = courseService;
        }

        public List<Course.Membership>? CourseList { get; set; }
        public User User { get; set; }
        public Course Course { get; set; }
        public CourseItem CourseListItem { get; set; }
        public class CourseItem
        {
            public Guid courseId { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid userId)
        {
            try
            {
                User = _userService.GetUser(userId);
                CourseList = _courseService.GetUserCourses(User.Id).ToList();

                return Page();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        //TODO: Add Search function to find course
        public async Task<IActionResult> OnPostAsync(Guid courseId, Guid userId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Course = _courseService.GetCourse(courseId);

                return RedirectToPage("./Details", new { userI = userId });
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
