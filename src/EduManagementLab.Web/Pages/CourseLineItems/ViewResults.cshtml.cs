using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.CourseLineItems
{
    public class ViewResultsModel : PageModel
    {
        private readonly CourseService _courseService;
        private readonly CourseLineItemService _courseLineItemService;
        public ViewResultsModel(CourseService courseService, CourseLineItemService courseLineItemsService)
        {
            _courseService = courseService;
            _courseLineItemService = courseLineItemsService;
        }
        public Course Course { get; set; }
        public CourseLineItem CourseLineItem { get; set; }

        [BindProperty]
        public List<UserScoreDto> userScoreList { get; set; } = new List<UserScoreDto>();
        [BindProperty]
        public UserScoreDto userScore { get; set; }
        public class UserScoreDto
        {
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
                Course = _courseService.GetCourse(courseId, true);
                CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId, true);
                var courseLineItem = CourseLineItem.Results.Where(c => c.CourseLineItemId == lineItemId);
                var userId = Guid.Parse(User?.GetSubjectId());

                foreach (var member in Course.Memperships.Where(c => c.UserId == userId))
                {
                    var result = courseLineItem.FirstOrDefault(r => r.MembershipId == member.Id);
                    if (result != null)
                    {
                        userScoreList.Add(new UserScoreDto
                        {                        
                            Firstname = member.User.FirstName,
                            Lastname = member.User.LastName,
                            lastUpdated = result.LastUpdated.ToString("f"),
                            Score = result.Score,
                            EndDate = member.EndDate
                        });
                    }
                }
                return Page();
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
