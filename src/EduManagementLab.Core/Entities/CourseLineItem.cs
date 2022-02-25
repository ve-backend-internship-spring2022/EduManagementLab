using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Core.Entities
{
    public class CourseLineItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public List<Result> Results { get; set; } = new List<Result>();


        public class Result
        {
            public Guid Id { get; set; }
            public User? User { get; set; }
            [Required]
            public Guid UserId { get; set; }
            public CourseLineItem? CourseLineItem { get; set; }
            [Required]
            public Guid CourseLineItemId { get; set; }
            public decimal Score { get; set; }
        }
    }
}
