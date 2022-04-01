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
        }

        public async Task<IActionResult> OnGetAsync(Guid lineItemId, Guid courseId)
        {
            try
            {
                PopulateProperties(lineItemId, courseId);
                return Page();
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }
        public IActionResult OnPostUpdateActive(Guid lineItemId, Guid courseId)
        {
            try
            {
                _courseLineItemService.UpdateCourseLineItemActive(lineItemId, IsChecked);
                PopulateProperties(lineItemId, courseId);
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
                var user = _userService.GetUser(userScore.UserId);
                CourseLineItem = _courseLineItemService.GetCourseLineItem(userScore.LineItemId, true);
                try
                {
                    if (user != null && CourseLineItem.Results.Any(x => x.UserId == userScore.UserId && x.CourseLineItemId == userScore.LineItemId) && userScore.Score != null)
                    {
                        var result = CourseLineItem.Results.FirstOrDefault(u => u.UserId == userScore.UserId && u.CourseLineItemId == userScore.LineItemId);
                        if (userScore.Score != result.Score && userScore.Score != null)
                        {
                            _courseLineItemService.UpdateLineItemResult(userScore.LineItemId, userScore.UserId, userScore.Score.Value);
                        }
                    }
                    else if (userScore.Score != null)
                    {
                        _courseLineItemService.CreateLineItemResult(userScore.LineItemId, userScore.UserId, userScore.Score.Value);
                    }
                }
                catch (CourseLineItemNotFoundException)
                {
                    return NotFound();
                }
            }
            PopulateProperties(lineItemId, courseId);
            return Page();
        }
        public IActionResult OnPostRemoveScore(Guid lineItemId, Guid courseId, Guid userId, decimal? score, int SelectedReason)
        {
            var course = _courseService.GetCourse(courseId, true);
            if (score != 0 && SelectedReason == 2)
            {
                try
                {
                    _courseLineItemService.DeleteLineItemResult(lineItemId, userId);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else if (score != 0 && SelectedReason == 1)
            {
                if (!course.Memperships.Any(u => u.UserId == userId))
                {
                    try
                    {
                        _courseLineItemService.DeleteLineItemResult(lineItemId, userId);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    var user = _userService.GetUser(userId);
                    ViewData["error"] = $"OBS! could not remove result. {user.FirstName} {user.LastName} is enrolled in this course";
                }
            }

            PopulateProperties(lineItemId, courseId);
            return Page();
        }
        private void PopulateProperties(Guid lineItemId, Guid courseId)
        {
            userScoreList.Clear();
            CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true);
            Course = _courseService.GetCourse(courseId, true);
            IsChecked = CourseLineItem.Active;

            var course = _courseService.GetCourse(courseId, true);
            var CourseLineItemResults = _courseLineItemService.GetCourseLineItem(lineItemId, true).Results.Where(c => c.CourseLineItemId == lineItemId);

            //loopa genom alla resultat
            foreach (var result in CourseLineItemResults)
            {
                if (!course.Memperships.Any(x => x.UserId == result.UserId))
                {
                    FetchUserScoreList(result.User, CourseLineItem);
                }
            }

            foreach (var user in course.Memperships)
            {
                FetchUserScoreList(user.User, CourseLineItem);
            }
        }
        private void FetchUserScoreList(User user, CourseLineItem courseLineItem)
        {
            decimal score = 0;
            string? lastupdated = "";
            if (CourseLineItem.Results.Any(u => u.UserId == user.Id))
            {
                var result = CourseLineItem.Results.FirstOrDefault(x => x.UserId == user.Id && x.CourseLineItemId == courseLineItem.Id);
                lastupdated = result.LastUpdated.ToString("f");
                score = result.Score;
            }

            userScoreList.Add(new UserScoreDto
            {
                UserId = user.Id,
                LineItemId = courseLineItem.Id,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Score = score,
                lastUpdated = lastupdated
            });
        }
        public PartialViewResult OnGetRemoveScoreModalPartial(Guid lineItemId, Guid courseId, Guid userId, decimal? score)
        {
            ViewData["lineItemId"] = lineItemId;
            ViewData["courseId"] = courseId;
            ViewData["userId"] = userId;
            ViewData["score"] = score;

            return new PartialViewResult
            {
                ViewName = "_RemoveScoreModalPartial",
                ViewData = new ViewDataDictionary(ViewData)
            };
        }
    }
}
