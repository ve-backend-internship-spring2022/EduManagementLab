using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.CourseTasks
{
    public class RemoveCourseTaskModel : PageModel
    {
        private readonly CourseTaskService _courseTaskService;
        public RemoveCourseTaskModel(CourseTaskService courseTaskService)
        {
            _courseTaskService = courseTaskService;
        }
        [BindProperty]
        public CourseTaskDto courseTaskDto { get; set; } = new CourseTaskDto();
        [BindProperty]
        public Guid courseId { get; set; }
        public class CourseTaskDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Active { get; set; }
        }
        public void OnGet(Guid courseTaskId, Guid courseId)
        {
            this.courseId = courseId;
            var courseTask = _courseTaskService.GetCourseTask(courseTaskId);
            courseTaskDto.Id = courseTask.Id;
            courseTaskDto.Name = courseTask.Name;
            courseTaskDto.Description = courseTask.Description;
        }
        public IActionResult OnPost(Guid courseId, Guid courseTaskId)
        {
            _courseTaskService.DeleteCourseTask(courseTaskId);
            return RedirectToPage("../Courses/Details", new { courseId = courseId });
        }
    }
}
