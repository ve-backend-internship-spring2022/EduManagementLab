using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer4.Extensions;

namespace EduManagementLab.Web.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly UserService _userService;
        public IndexModel(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserDto user { get; set; } = new UserDto();
        public class UserDto
        {
            public string UserName { get; set; }
            public string Displayname { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }
        public IActionResult OnGet()
        {
            var userId = Guid.Parse(User?.GetSubjectId());
            var targetUser = _userService.GetUser(userId);
            user.UserName = targetUser.UserName;
            user.FirstName = targetUser.FirstName;
            user.LastName = targetUser.LastName;
            user.Email = targetUser.Email;
            user.Displayname = targetUser.Displayname;
            return Page();
        }
    }
}
