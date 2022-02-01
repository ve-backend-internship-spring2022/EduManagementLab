#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EduManagementLab.Core.Services;
using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;

namespace EduManagementLab.Web.Pages.Users
{
    public class DeleteUserModel : PageModel
    {
        private readonly UserService _userService;

        public DeleteUserModel(UserService userService)
        {
            _userService = userService;
        }

        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid userId)
        {
            try
            {
                User = _userService.GetUser(userId);
                return Page();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid userId)
        {
            try
            {
                _userService.DeleteUser(userId);
                return RedirectToPage("./Index");
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
