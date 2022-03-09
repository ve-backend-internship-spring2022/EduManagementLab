using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace EduManagementLab.Web.Pages.CourseLineItems
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

        public CourseLineItem CourseLineItem { get; set; }
        public SelectList ResultListItems { get; set; }
        [BindProperty]
        public bool IsChecked { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid lineItemId)
        {
            try
            {
                PopulateProperties(lineItemId);
                return Page();
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }
        public IActionResult OnPost(Guid lineItemId)
        {
            try
            {
                _courseLineItemService.UpdateCourseLineItemActive(lineItemId, IsChecked);
                PopulateProperties(lineItemId);
                return Page();
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }
        private void PopulateProperties(Guid lineItemId)
        {
            CourseLineItem = _courseLineItemService.GetCourseLineItem(lineItemId);
            IsChecked = CourseLineItem.Active;
            ResultListItems = new SelectList(_courseLineItemService.GetLineItemResults(lineItemId));
        }
    }
}
