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
    public class DeleteModel : PageModel
    {
        private readonly UserService _userService;

        public DeleteModel(UserService userService)
        {
            _userService = userService;
        }

        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                User = _userService.GetUser(id);
                return Page();
            }
            catch (UserNotFoundException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            try
            {
                _userService.DeleteUser(id);
                return RedirectToPage("./Index");
            }
            catch (UserNotFoundException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return Page();
            }
        }
    }
}
