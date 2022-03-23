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
        public decimal Score { get; set; }
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

        public IActionResult OnPostUpdateScore(Guid lineItemId, Guid userId, Guid courseId)
        {
            var user = _userService.GetUser(userId);
            CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true);

            try
            {
                if (user != null && CourseLineItem.Results.Any(x => x.UserId == userId && x.CourseLineItemId == lineItemId))
                {
                    _courseLineItemService.UpdateLineItemResult(lineItemId, userId, Score);
                }
                else
                {
                    _courseLineItemService.CreateLineItemResult(lineItemId, userId, Score);
                }
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }

            PopulateProperties(lineItemId, courseId);
            return Page();
        }

        private void PopulateProperties(Guid lineItemId, Guid courseId)
        {
            CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true);
            Course = _courseService.GetCourse(courseId, true);
            IsChecked = CourseLineItem.Active;
            decimal score = 0;

            var memberships = _courseService.GetCourse(courseId, true);
            var users = _userService.GetUsers();

            //loopa genom alla användare
            foreach (var userNotMembership in users)
            {
                // kolla om användare har resulatat men inte är medlem och om medlemmar redan finns
                if (!memberships.Memperships.Any(x => x.UserId == userNotMembership.Id)
                    && CourseLineItem.Results.Any(u => u.UserId == userNotMembership.Id))
                {
                    //Hämta resultat för användare som är borttagna från Medlemskap fast har betyg kvar
                    score = CourseLineItem.Results.FirstOrDefault(x => x.UserId == userNotMembership.Id && x.CourseLineItemId == lineItemId).Score;
                    userScoreList.Add(new UserScoreDto
                    {
                        UserId = userNotMembership.Id,
                        LineItemId = lineItemId,
                        Firstname = userNotMembership.FirstName,
                        Lastname = userNotMembership.LastName,
                        Score = score,
                    });
                }
            }

            foreach (var user in memberships.Memperships)
            {
                //Kolla om användare har resultat
                if (CourseLineItem.Results.Any(u => u.UserId == user.UserId))
                {
                    //Hämta score för användare som är Medlem
                    score = CourseLineItem.Results.FirstOrDefault(x => x.UserId == user.UserId && x.CourseLineItemId == lineItemId).Score;
                }
                userScoreList.Add(new UserScoreDto
                {
                    UserId = user.UserId,
                    LineItemId = lineItemId,
                    Firstname = user.User.FirstName,
                    Lastname = user.User.LastName,
                    Score = score,
                });
                //_courseLineItemService.CreateLineItemResult(lineItemId, user.UserId, 0);
            }
        }
    }
}
