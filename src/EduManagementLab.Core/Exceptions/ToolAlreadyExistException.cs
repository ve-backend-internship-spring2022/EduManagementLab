using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class ToolAlreadyExistException : Exception
    {
        public ToolAlreadyExistException(Guid toolId) : base($"Tool already exist with id {toolId}")
        {
        }
    }
}
