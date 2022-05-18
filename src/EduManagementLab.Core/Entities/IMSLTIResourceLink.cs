using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Entities
{
    public class IMSLTIResourceLink
    {
        public Guid Id { get; set; }
        public string? CustomProperties { get; set; }
        public string? Description { get; set; }
        public string Title { get; set; }
        public Tool Tool { get; set; }
    }
}
