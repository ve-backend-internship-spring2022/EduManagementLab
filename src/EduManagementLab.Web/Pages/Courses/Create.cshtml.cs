using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Courses
{
    public class CreateModel : PageModel
    {
        private readonly CourseService _courseService;

        public CreateModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        public Course Course { get; set; }
        [BindProperty]
        [Required]
        public string Code { get; set; }
        [BindProperty]
        [Required]
        public string Name { get; set; }
        [BindProperty]
        public string? Description { get; set; }
        [BindProperty]
        [Required]
        public DateTime StartDate { get; set; }
        [BindProperty]
        [Required]
        public DateTime EndDate { get; set; }
        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _courseService.CreateCourse(Code, Name, Description, StartDate, EndDate);
                    return RedirectToPage("./Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(String.Empty, "Course code/name already exist");
                    return Page();
                }
            }
            return Page();
        }
    }
}
