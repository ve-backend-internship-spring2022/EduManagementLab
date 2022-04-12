#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace EduManagementLab.Web.Pages.Users
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly UserService _userService;

        public CreateModel(UserService userService)
        {
            _userService = userService;
        }

        [Required]
        [RegularExpression("^[A-Za-z][A-Za-z0-9_]{7,29}$", ErrorMessage = "Invalid")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Displayname { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _userService.CreateUser(Password, Username, Displayname, FirstName, LastName, Email);
                    return RedirectToPage("./Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Email", "User already exist");
                    return Page();
                }
            }
            return Page();
        }
    }
}
