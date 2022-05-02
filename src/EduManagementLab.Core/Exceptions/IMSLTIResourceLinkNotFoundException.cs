using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class IMSLTIResourceLinkNotFoundException : Exception
    {
        public IMSLTIResourceLinkNotFoundException(Guid resourceId) : base($"No resourceLink found with id {resourceId}")
        {
        }
    }
}
