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
            filterList.Add(new SelectListItem() { Text = "None", Value = "0" });
            filterList.Add(new SelectListItem() { Text = "Not member but have results", Value = "1" });
            filterList.Add(new SelectListItem() { Text = "Member have results", Value = "2" });
            filterList.Add(new SelectListItem() { Text = "Member have no results", Value = "3" });
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
            PopulateProperties(lineItemId, courseId, selectedFilter);
            return Page();
        }
        public IActionResult OnPostFilter(Guid lineItemId, Guid courseId)
        {
            PopulateProperties(lineItemId, courseId, selectedFilter);
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

            PopulateProperties(lineItemId, courseId, selectedFilter);
            return Page();
        }
        private void PopulateProperties(Guid lineItemId, Guid courseId, int selectedFilter)
        {
            userScoreList.Clear();
            CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true);
            Course = _courseService.GetCourse(courseId, true);
            IsChecked = CourseLineItem.Active;

            var course = _courseService.GetCourse(courseId, true);
            var courseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true).Results.Where(c => c.CourseLineItemId == lineItemId);

            switch (selectedFilter)
            {
                case 1:
                    LoadResult(false, true, course, courseLineItem);
                    break;
                case 2:
                    LoadResult(true, true, course, courseLineItem);
                    break;
                case 3:
                    LoadResult(true, false, course, courseLineItem);
                    break;
                default:
                    LoadResult(null, null, course, courseLineItem);
                    break;
            }
            loadFilters();
        }
        private void LoadResult(bool? isMember, bool? haveResult, Course course, IEnumerable<CourseLineItem.Result> courselineitemResult)
        {
            // if user is not member but have result and display it on web
            if (isMember == false && haveResult == true)
            {
                foreach (var result in courselineitemResult)
                {
                    if (!course.Memperships.Any(x => x.UserId == result.UserId) && CourseLineItem.Results.Any(u => u.UserId == result.UserId && u.CourseLineItemId == result.CourseLineItemId))
                    {
                        FillUserScoreList(result);
                    }
                }
            }
            // if user is member and have result and display it on web
            else if (isMember == true && haveResult == true)
            {
                foreach (var result in courselineitemResult)
                {
                    if (course.Memperships.Any(x => x.UserId == result.UserId))
                    {
                        FillUserScoreList(result);
                    }
                }
            }
            // if user is member but have no result and display it on web
            else if (isMember == true && haveResult == false)
            {
                foreach (var user in course.Memperships)
                {
                    if (!CourseLineItem.Results.Any(u => u.UserId == user.UserId && u.CourseLineItemId == CourseLineItem.Id))
                    {
                        userScoreList.Add(new UserScoreDto
                        {
                            UserId = user.UserId,
                            LineItemId = CourseLineItem.Id,
                            Firstname = user.User.FirstName,
                            Lastname = user.User.LastName,
                        });
                    }
                }
            }
            // else get all result and member and display it on web
            else
            {
                foreach (var user in course.Memperships)
                {
                    if (!CourseLineItem.Results.Any(u => u.UserId == user.UserId && u.CourseLineItemId == CourseLineItem.Id))
                    {
                        userScoreList.Add(new UserScoreDto
                        {
                            UserId = user.UserId,
                            LineItemId = CourseLineItem.Id,
                            Firstname = user.User.FirstName,
                            Lastname = user.User.LastName,
                        });
                    }
                    else
                    {
                        var userResult = CourseLineItem.Results.FirstOrDefault(x => x.UserId == user.UserId && x.CourseLineItemId == CourseLineItem.Id);
                        FillUserScoreList(userResult);
                    }
                }
                foreach (var result in courselineitemResult)
                {
                    if (!course.Memperships.Any(x => x.UserId == result.UserId))
                    {
                        FillUserScoreList(result);
                    }
                }
            }
        }
        private void FillUserScoreList(CourseLineItem.Result courseLineItemResult)
        {
            userScoreList.Add(new UserScoreDto
            {
                UserId = courseLineItemResult.UserId,
                LineItemId = courseLineItemResult.Id,
                Firstname = courseLineItemResult.User.FirstName,
                Lastname = courseLineItemResult.User.LastName,
                Score = courseLineItemResult.Score,
                lastUpdated = courseLineItemResult.LastUpdated.ToString("f")
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
