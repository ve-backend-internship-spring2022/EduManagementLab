using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult OnPost(Guid lineItemId, Guid courseId)
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
        public IActionResult OnPostUpdateScore(Guid lineItemId, Guid courseId, Guid userId)
        {
            foreach (var userScore in userScoreList)
            {
                var user = _userService.GetUser(userScore.UserId);
                CourseLineItem = _courseLineItemService.GetCourseLineItem(userScore.LineItemId, true);
                try
                {
                    if (user != null && CourseLineItem.Results.Any(x => x.UserId == userScore.UserId && x.CourseLineItemId == userScore.LineItemId) && userScore.Score != null)
                    {
                        _courseLineItemService.UpdateLineItemResult(userScore.LineItemId, userScore.UserId, userScore.Score.Value);
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
        private void PopulateProperties(Guid lineItemId, Guid courseId)
        {
            userScoreList.Clear();
            CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true);
            Course = _courseService.GetCourse(courseId, true);
            IsChecked = CourseLineItem.Active;

            var course = _courseService.GetCourse(courseId, true);
            var users = _userService.GetUsers();

            //loopa genom alla användare
            foreach (var userNotMembership in users)
            {
                // kolla om användare har resulatat men inte är medlem och om medlemmar redan finns
                if (!course.Memperships.Any(x => x.UserId == userNotMembership.Id)
                    && CourseLineItem.Results.Any(u => u.UserId == userNotMembership.Id))
                {
                    FetchUserScoreList(userNotMembership, CourseLineItem, false);
                }
            }

            foreach (var user in course.Memperships)
            {
                FetchUserScoreList(user.User, CourseLineItem, true);
            }
        }
        private void FetchUserScoreList(User user, CourseLineItem courseLineItem, bool isMember)
        {
            decimal score = 0;
            if (CourseLineItem.Results.Any(u => u.UserId == user.Id) && isMember == true)
            {
                score = CourseLineItem.Results.FirstOrDefault(x => x.UserId == user.Id && x.CourseLineItemId == courseLineItem.Id).Score;
            }
            else if(CourseLineItem.Results.Any(u => u.UserId == user.Id) &&isMember == false )
            {
                score = CourseLineItem.Results.FirstOrDefault(x => x.UserId == user.Id && x.CourseLineItemId == courseLineItem.Id).Score;
            }

            userScoreList.Add(new UserScoreDto
            {
                UserId = user.Id,
                LineItemId = courseLineItem.Id,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Score = score,
            });
        }
    }
}
