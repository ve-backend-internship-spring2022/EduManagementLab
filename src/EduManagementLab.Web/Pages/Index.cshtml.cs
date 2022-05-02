using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserService _userService;
        private readonly CourseService _courseService;
        private readonly CourseTaskService _courseLineItemService;

        public IndexModel(ILogger<IndexModel> logger, UserService userService, CourseService courseService, CourseTaskService courseLineItemService)
        {
            _logger = logger;
            _userService = userService;
            _courseService = courseService;
            _courseLineItemService = courseLineItemService;
        }
        [BindProperty]
        public Guid LoginUserId { get; set; }

        [BindProperty]
        public User UserLoggedIn { get; set; }
        public List<Course.Membership>? CourseList { get; set; }

        public List<CourseMembershipDto> ListOfCourseMemberships { get; set; } = new List<CourseMembershipDto>();
        [BindProperty]
        public CourseMembershipDto CourseMembership_Dto { get; set; } = new CourseMembershipDto();
        public class CourseMembershipDto
        {
            public Guid Id { get; set; }
            [Required]
            public string Code { get; set; }
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; }
            [Required]
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                LoginUserId = Guid.Parse(User?.GetSubjectId());
                UserLoggedIn = _userService.GetUser(LoginUserId);
                CourseList = _courseService.GetUserCourses(UserLoggedIn.Id).ToList();

                foreach (var member in CourseList)
                {
                    ListOfCourseMemberships.Add(new CourseMembershipDto()
                    {
                        Id = member.CourseId,
                        Name = member.Course.Name,
                        Code = member.Course.Code,
                        Description = member.Course.Description,
                        StartDate = member.Course.StartDate,
                        EndDate = member.EndDate
                    });
                }

                return Page();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
    }
}