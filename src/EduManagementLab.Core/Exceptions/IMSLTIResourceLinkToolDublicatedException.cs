using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class IMSLTIResourceLinkToolDublicatedException : Exception
    {
        public IMSLTIResourceLinkToolDublicatedException(string tool) : base($"This ResourceLink {tool} which is conflict with another ResourceLink!")
        {
        }
    }
}
