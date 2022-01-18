using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Memberships
{
    public class CreateModel : PageModel
    {
        private readonly CourseService _courseService;

        public CreateModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        [Required]
        public Course Course { get; set; }
        [BindProperty]
        [Required]
        public User User { get; set; }
        [BindProperty]
        [Required]
        public DateTime EnrolledDate { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    _courseService.CreateCourseMembership(Course, User, EnrolledDate);

        //    return RedirectToPage("./Index");
        //}
    }
}
