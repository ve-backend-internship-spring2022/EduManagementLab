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
    public class EditNameModel : PageModel
    {
        private readonly UserService _userService;

        public EditNameModel(UserService userService)
        {
            _userService = userService;
        }
        [BindProperty]
        [Required]
        public Guid UserId { get; set; }
        [BindProperty]
        [Required]
        public string Displayname { get; set; }
        [BindProperty]
        [Required]
        public string FirstName { get; set; }
        [BindProperty]
        [Required]
        public string LastName { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var user = _userService.GetUser(id);
                UserId = user.Id;
                Displayname = user.Displayname;
                FirstName = user.FirstName;
                LastName = user.LastName;

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
                _userService.UpdateName(UserId, Displayname, FirstName, LastName);
                return RedirectToPage("./Index");
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
