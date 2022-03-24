using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.CourseLineItems
{
    public class RemoveCourseLineItemResultModel : PageModel
    {
        private readonly CourseLineItemService _courseLineItemService;
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        public RemoveCourseLineItemResultModel(CourseLineItemService courseLineItemService, UserService userService, CourseService courseService)
        {
            _courseLineItemService = courseLineItemService;
            _userService = userService;
            _courseService = courseService;
        }

        [BindProperty]
        public UserScoreDto userResultDto { get; set; } = new UserScoreDto();
        public class UserScoreDto
        {
            public Guid UserId { get; set; }
            public Guid LineItemId { get; set; }
            public Guid CourseId { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public decimal? Score { get; set; }
        }
        public void OnGet(Guid courseId, Guid userId, Guid lineItemId)
        {
            var courseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true);
            var user = _userService.GetUser(userId);

            if (courseLineItem.Results.Any(x => x.UserId == userId && x.CourseLineItemId == lineItemId))
            {
                userResultDto.Score = courseLineItem.Results.FirstOrDefault(x => x.UserId == userId && x.CourseLineItemId == lineItemId).Score;
            }
            userResultDto.CourseId = courseId;
            userResultDto.UserId = user.Id;
            userResultDto.LineItemId = lineItemId;
            userResultDto.Firstname = user.FirstName;
            userResultDto.Lastname = user.LastName;
        }

        public IActionResult OnPost(Guid userId, Guid lineItemId, Guid courseId)
        {
            _courseLineItemService.DeleteLineItemResult(lineItemId, userId);
            return RedirectToPage("../CourseLineItems/Details", new { lineItemId = lineItemId, courseId = courseId });
        }
    }
}
