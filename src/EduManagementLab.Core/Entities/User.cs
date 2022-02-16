using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        [RegularExpression(@"^[åäöÅÄÖA-Za-z_]*$",
            ErrorMessage = "Username can only contain alphanumeric character")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "Display name must be between 3 and 50 characters")]
        public string Displayname { get; set; }
        [Required]
        [RegularExpression(@"^[åäöÅÄÖa-zA-Z0-9]*$",
        ErrorMessage = "First name can only contain letters")]
        [StringLength(100, MinimumLength = 2,
        ErrorMessage = "First name must be between 2 and 100 characters")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[åäöÅÄÖA-Za-z_]*$",
        ErrorMessage = "Last name can only contain letters")]
        [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Display name must be between 2 and 100 characters")]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
        ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

    }
}
