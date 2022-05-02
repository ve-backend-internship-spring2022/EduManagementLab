using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.CourseTasks
{
    public class EditCourseTaskInfoModel : PageModel
    {
        private readonly CourseTaskService _courseTaskService;
        public EditCourseTaskInfoModel(CourseTaskService courseTaskService)
        {
            _courseTaskService = courseTaskService;
        }
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        [Required]
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string? Description { get; set; }

        public void OnGet(Guid id, Guid courseId)
        {
            var courseTask = _courseTaskService.GetCourseTask(id);
            CourseId = courseId;
            Id = courseTask.Id;
            Name = courseTask.Name;
            Description = courseTask.Description;
        }
        public IActionResult OnPost(Guid id, Guid courseId)
        {
            if (ModelState.IsValid)
            {
                _courseTaskService.UpdateCourseTask(id, Name, Description);
            }
            return RedirectToPage("./Details", new { courseTaskId = id, courseId = courseId });
        }
    }
}
