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
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public SelectList CourseListItems { get; set; }
        public User User { get; set; }
        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid userId)
        {
            try
            {
                User = _userService.GetUser(userId);
                CourseList = _courseService.GetUserCourses(User.Id).ToList();

                CourseListItems = new SelectList(_courseService.GetCourses()
                    .Where(s => !CourseList.Any(x => x.Course.Name == s.Name)), "Id", "Name");

                return Page();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> OnPostAsync(Guid Id, Guid userId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _courseService.CreateCourseMembership(Id, userId, DateTime.Now);
                return RedirectToPage("./Details", new { userId = userId });
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
