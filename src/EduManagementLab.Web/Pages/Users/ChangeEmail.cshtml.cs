#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EduManagementLab.Core.Services;
using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Users
{
    public class ChangeEmailModel : PageModel
    {
        private readonly UserService _userService;

        public ChangeEmailModel(UserService userService)
        {
            _userService = userService;
        }
        [BindProperty]
        [Required]
        public Guid UserId { get; set; }
        [BindProperty]
        [Required]
        public string Email { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var user = _userService.GetUser(id);
                UserId = user.Id;
                Email = user.Email;

                return Page();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }

        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _userService.UpdateEmail(UserId, Email);
                return RedirectToPage("./Details", new { userId = UserId });
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
