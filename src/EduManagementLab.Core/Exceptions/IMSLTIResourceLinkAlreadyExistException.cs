using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class IMSLTIResourceLinkAlreadyExistException : Exception
    {
        public IMSLTIResourceLinkAlreadyExistException(string resourceLink) : base($"This ResourceLink {resourceLink} already Exist!")
        {
        }
    }
}
