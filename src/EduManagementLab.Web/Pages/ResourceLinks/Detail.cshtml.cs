using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.ResourceLinks
{
    public class DetailModel : PageModel
    {
        private readonly ResourceLinkService _resourceLinkService;
        public DetailModel(ResourceLinkService resourceLinkService)
        {
            _resourceLinkService = resourceLinkService;
        }
        [BindProperty]
        public IMSLTIResourceLink ResourceLink { get; set; } = new IMSLTIResourceLink();
        [BindProperty]
        public Guid CourseTaskId { get; set; }
        public IActionResult OnGet(Guid resourceId, Guid courseTaskId)
        {
            CourseTaskId = courseTaskId;
            try
            {
                ResourceLink = _resourceLinkService.GetResourceLink(resourceId);
            }
            catch (IMSLTIResourceLinkNotFoundException)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
