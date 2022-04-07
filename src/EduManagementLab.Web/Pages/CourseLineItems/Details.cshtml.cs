using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;


namespace EduManagementLab.Web.Pages.CourseLineItems
{
    public class DetailsModel : PageModel
    {
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        private readonly CourseLineItemService _courseLineItemService;
        public DetailsModel(CourseService courseService, UserService userService, CourseLineItemService courseLineItemService)
        {
            _courseService = courseService;
            _userService = userService;
            _courseLineItemService = courseLineItemService;
        }
        [BindProperty]
        public List<SelectListItem> filterList { get; } = new List<SelectListItem>();
        [BindProperty]
        public int selectedFilter { get; set; }
        public Course Course { get; set; }
        public CourseLineItem CourseLineItem { get; set; }
        public SelectList UserListItems { get; set; }
        [BindProperty]
        public bool IsChecked { get; set; }
        [BindProperty]
        public List<UserScoreDto> userScoreList { get; set; } = new List<UserScoreDto>();
        [BindProperty]
        public UserScoreDto userScore { get; set; }
        public class UserScoreDto
        {
            public Guid UserId { get; set; }
            public Guid LineItemId { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string? lastUpdated { get; set; }
            public decimal? Score { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid lineItemId, Guid courseId)
        {
            try
            {
                PopulateProperties(lineItemId, courseId, selectedFilter);
                return Page();
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }
        private void loadFilters()
        {
            filterList.Add(new SelectListItem() { Text = "Active Members", Value = "0" });
            filterList.Add(new SelectListItem() { Text = "Completed Members", Value = "1" });
        }
        public IActionResult OnPostUpdateActive(Guid lineItemId, Guid courseId)
        {
            try
            {
                _courseLineItemService.UpdateCourseLineItemActive(lineItemId, IsChecked);
                PopulateProperties(lineItemId, courseId, selectedFilter);
                return Page();
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }
        public IActionResult OnPostUpdateScore(Guid lineItemId, Guid courseId)
        {
            foreach (var userScore in userScoreList)
            {
                var course = _courseService.GetCourse(courseId, true);
                var member = course.Memperships.FirstOrDefault(x => x.UserId == userScore.UserId);

                CourseLineItem = _courseLineItemService.GetCourseLineItem(userScore.LineItemId, true);
                try
                {
                    if (member != null && CourseLineItem.Results.Any(x => x.MembershipId == member.Id && x.CourseLineItemId == userScore.LineItemId) && userScore.Score != null)
                    {
                        var result = CourseLineItem.Results.FirstOrDefault(u => u.MembershipId == member.Id && u.CourseLineItemId == userScore.LineItemId);
                        if (userScore.Score != result.Score && userScore.Score != null)
                        {
                            _courseLineItemService.UpdateLineItemResult(userScore.LineItemId, member.Id, userScore.Score.Value);
                        }
                    }
                    else if (userScore.Score != null)
                    {
                        _courseLineItemService.CreateLineItemResult(userScore.LineItemId, member.Id, userScore.Score.Value);
                    }
                }
                catch (CourseLineItemNotFoundException)
                {
                    return NotFound();
                }
            }
            PopulateProperties(lineItemId, courseId, selectedFilter);
            return Page();
        }
        public IActionResult OnPostFilter(Guid lineItemId, Guid courseId)
        {
            PopulateProperties(lineItemId, courseId, selectedFilter);
            return Page();
        }
        private void PopulateProperties(Guid lineItemId, Guid courseId, int selectedFilter)
        {
            userScoreList.Clear();
            CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true);
            Course = _courseService.GetCourse(courseId, true);
            IsChecked = CourseLineItem.Active;

            var courseLineItemResult = CourseLineItem.Results.Where(c => c.CourseLineItemId == lineItemId);

            switch (selectedFilter)
            {
                case 0:
                    var activeUsers = Course.Memperships.Where(s => s.EndDate == null);
                    FillUserScoreList(lineItemId, activeUsers, courseLineItemResult);
                    break;
                case 1:
                    var inactiveUsers = Course.Memperships.Where(s => s.EndDate != null);
                    FillUserScoreList(lineItemId, inactiveUsers, courseLineItemResult);
                    break;
                default:
                    FillUserScoreList(lineItemId, Course.Memperships, courseLineItemResult);
                    break;
            }
            loadFilters();
        }
        private void FillUserScoreList(Guid lineitemId, IEnumerable<Course.Membership> memberships, IEnumerable<CourseLineItem.Result> results)
        {
            foreach (var member in memberships)
            {
                var result = results.FirstOrDefault(r => r.MembershipId == member.Id);
                if (result != null)
                {
                    userScoreList.Add(new UserScoreDto
                    {
                        UserId = member.UserId,
                        LineItemId = lineitemId,
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
                        LineItemId = lineitemId,
                        Firstname = member.User.FirstName,
                        Lastname = member.User.LastName,
                        EndDate = member.EndDate
                    });
                }
            }

        }
    }
}
