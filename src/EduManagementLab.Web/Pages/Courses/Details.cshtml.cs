using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Courses
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
        public SelectList UserListItems { get; set; }
        public SelectList LineItemListItems { get; set; }

        [BindProperty]
        public DateTime EnrolledDate { get; set; }
        [BindProperty]
        public LineItemInputModel lineItemInput { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        [BindProperties]
        public class InputModel
        {
            public Guid Id { get; set; }
            [Required(ErrorMessage = "Displayname Required!")]
            public string DisplayName { get; set; }
            [Required(ErrorMessage = "Firstname Required!")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "Lastname Required!")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "Email Required!")]
            [DataType(DataType.EmailAddress, ErrorMessage = "Please type a valid email address")]
            public string Email { get; set; }
        }
        public class LineItemInputModel
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Description { get; set; }
            [Required]
            public bool Active { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid courseId)
        {
            try
            {
                PopulateProperties(courseId);
                return Page();
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }

        private void PopulateProperties(Guid courseId)
        {
            Course = _courseService.GetCourse(courseId, true);

            UserListItems = new SelectList(_userService.GetUsers()
                .Where(s => !Course.Memperships.Any(x => x.User.Email == s.Email)), "Id", "Email");

            LineItemListItems = new SelectList(_courseLineItemService.GetCourseLineItems()
               .Where(s => !Course.CourseLineItems.Any(x => x.Name == s.Name)), "Name", "Description");
        }

        public async Task<IActionResult> OnPostExistingUserAsync(Guid courseId, Guid Id)
        {
            try
            {
                var existingUser = _userService.GetUser(Id);
                _courseService.CreateCourseMembership(courseId, existingUser.Id, DateTime.Now);

                return RedirectToPage("./Details", new { courseId = courseId });
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostNewUserAsync(Guid courseId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newUser = _userService.CreateUser(Input.DisplayName, Input.FirstName, Input.LastName, Input.Email);
                    _courseService.CreateCourseMembership(courseId, newUser.Id, DateTime.Now);

                    return RedirectToPage("./Details", new { courseId = courseId });
                }
                catch (UserAlreadyExistException)
                {
                    ModelState.AddModelError("Input.Email", "User already exist");
                    ViewData["ShowCreateModal"] = !string.IsNullOrEmpty(Input.Email) ? true : false;
                    PopulateProperties(courseId);
                    return Page();
                }
            }
            else
            {
                PopulateProperties(courseId);
                return Page();
            }
        }
        public IActionResult OnPostCreateLineItem(Guid courseId)
        {
            ICollection<ValidationResult> results = null;
            if (Validate(lineItemInput, out results))
            {
                _courseLineItemService.CreateCourseLineItem(courseId, lineItemInput.Name, lineItemInput.Description, lineItemInput.Active);
            }
            PopulateProperties(courseId);
            return RedirectToPage("./Details", new { courseId = courseId });
        }
        private bool Validate<T>(T obj, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, new ValidationContext(obj), results, true);
        }
    }
}
