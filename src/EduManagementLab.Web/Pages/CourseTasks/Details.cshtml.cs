using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;


namespace EduManagementLab.Web.Pages.CourseTasks
{

    [BindProperties]
    public class DetailsModel : PageModel
    {
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        private readonly CourseTaskService _courseTaskService;
        public DetailsModel(CourseService courseService, UserService userService, CourseTaskService courseTaskService)
        {
            _courseService = courseService;
            _userService = userService;
            _courseTaskService = courseTaskService;
        }
        public List<SelectListItem> filterList { get; } = new List<SelectListItem>();
        public List<IMSLTIResourceLink> ResourceLinks { get; } = new List<IMSLTIResourceLink>();

        public int selectedFilter { get; set; }
        public Course Course { get; set; }
        public CourseTask CourseTask { get; set; }
        public SelectList UserListItems { get; set; }
        public List<UserScoreDto> userScoreList { get; set; } = new List<UserScoreDto>();
        public UserScoreDto userScore { get; set; }
        public class UserScoreDto
        {
            public Guid UserId { get; set; }
            public Guid CourseTaskId { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string? lastUpdated { get; set; }
            public decimal? Score { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid courseTaskId, Guid courseId)
        {
            try
            {
                PopulateProperties(courseTaskId, courseId, selectedFilter);
                return Page();
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }
        private void loadFilters()
        {
            filterList.Add(new SelectListItem() { Text = "Active Members", Value = "0" });
            filterList.Add(new SelectListItem() { Text = "Completed Members", Value = "1" });
        }
        public IActionResult OnPostUpdateScore(Guid courseTaskId, Guid courseId)
        {
            foreach (var userScore in userScoreList)
            {
                var course = _courseService.GetCourse(courseId, true);
                var member = course.Memperships.FirstOrDefault(x => x.UserId == userScore.UserId);

                CourseTask = _courseTaskService.GetCourseTask(userScore.CourseTaskId, true);
                try
                {
                    if (member != null && CourseTask.Results.Any(x => x.MembershipId == member.Id && x.CourseTaskId == userScore.CourseTaskId) && userScore.Score != null)
                    {
                        var result = CourseTask.Results.FirstOrDefault(u => u.MembershipId == member.Id && u.CourseTaskId == userScore.CourseTaskId);
                        if (userScore.Score != result.Score && userScore.Score != null)
                        {
                            _courseTaskService.UpdateCourseTaskResult(userScore.CourseTaskId, member.Id, userScore.Score.Value);
                        }
                    }
                    else if (userScore.Score != null)
                    {
                        _courseTaskService.CreateCourseTaskResult(userScore.CourseTaskId, member.Id, userScore.Score.Value);
                    }
                }
                catch (CourseTaskNotFoundException)
                {
                    return NotFound();
                }
            }
            PopulateProperties(courseTaskId, courseId, selectedFilter);
            return Page();
        }
        public IActionResult OnPostFilter(Guid courseTaskId, Guid courseId)
        {
            PopulateProperties(courseTaskId, courseId, selectedFilter);
            return Page();
        }
        private void PopulateProperties(Guid courseTaskId, Guid courseId, int selectedFilter)
        {
            userScoreList.Clear();
            CourseTask = _courseTaskService.GetCourseTask(courseTaskId, true);
            Course = _courseService.GetCourse(courseId, true);

            var courseTaskResult = CourseTask.Results.Where(c => c.CourseTaskId == courseTaskId);

            switch (selectedFilter)
            {
                case 0:
                    var activeUsers = Course.Memperships.Where(s => s.EndDate == null);
                    FillUserScoreList(courseTaskId, activeUsers, courseTaskResult);
                    break;
                case 1:
                    var inactiveUsers = Course.Memperships.Where(s => s.EndDate != null);
                    FillUserScoreList(courseTaskId, inactiveUsers, courseTaskResult);
                    break;
                default:
                    FillUserScoreList(courseTaskId, Course.Memperships, courseTaskResult);
                    break;
            }
            loadFilters();
        }
        private void FillUserScoreList(Guid courseTaskId, IEnumerable<Course.Membership> memberships, IEnumerable<CourseTask.Result> results)
        {
            foreach (var member in memberships)
            {
                var result = results.FirstOrDefault(r => r.MembershipId == member.Id);
                if (result != null)
                {
                    userScoreList.Add(new UserScoreDto
                    {
                        UserId = member.UserId,
                        CourseTaskId = courseTaskId,
                        Firstname = member.User.FirstName,
                        Lastname = member.User.LastName,
                        lastUpdated = result.LastUpdated.ToString("f"),
                        Score = result.Score,
                        EndDate = member.EndDate
                    });
                }
                else
                {
                    userScoreList.Add(new UserScoreDto
                    {
                        UserId = member.UserId,
                        CourseTaskId = courseTaskId,
                        Firstname = member.User.FirstName,
                        Lastname = member.User.LastName,
                        EndDate = member.EndDate
                    });
                }
            }

        }
    }
}
