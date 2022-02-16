using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        [Required]
        [RegularExpression(@"^[åäöÅÄÖa-zA-Z0-9]*$",
        ErrorMessage = "Course codes can only contain alphanumeric character")]
        [StringLength(10, MinimumLength = 3,
        ErrorMessage = "Course code must be between 3 and 10 characters")]
        public string Code { get; set; }
        [Required]
        [RegularExpression(@"^[åäöÅÄÖa-zA-Z0-9]*$",
        ErrorMessage = "Course name can only contain letters")]
        [StringLength(50, MinimumLength = 5,
        ErrorMessage = "Course name must be between 5 and 50 characters")]
        public string Name { get; set; }
        [RegularExpression(@"^[åäöÅÄÖa-zA-Z0-9]*$",
        ErrorMessage = "Course description can only contain letters")]
        [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Course description must be between 5 and 100 characters")]
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public List<Membership> Memperships { get; set; } = new List<Membership>();

        public class Membership
        {
            public Guid Id { get; set; }
            public Course? Course { get; set; }
            [Required]
            public Guid CourseId { get; set; }
            public User? User { get; set; }
            [Required]
            public Guid UserId { get; set; }
            [Required]
            public DateTime EnrolledDate { get; set; }
        }
    }
}
