using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduManagementLab.Core.Entities
{
    public class CourseTask
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<Result> Results { get; set; } = new List<Result>();
        public class Result
        {
            public Guid Id { get; set; }
            public Course.Membership? Membership { get; set; }
            [Required]
            public Guid MembershipId { get; set; }
            public CourseTask? CourseTask { get; set; }
            [Required]
            public Guid CourseTaskId { get; set; }
            [Column(TypeName = "decimal")]
            public decimal Score { get; set; }
            public DateTime LastUpdated { get; set; }
        }
    }
}
