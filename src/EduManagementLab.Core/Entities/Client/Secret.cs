using System;

namespace EduManagementLab.Core.Entities.client
{
    public abstract class Secret
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; } = "SharedSecret";
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}