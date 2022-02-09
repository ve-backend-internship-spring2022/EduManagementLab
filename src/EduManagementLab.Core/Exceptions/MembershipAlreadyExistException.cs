using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class MembershipAlreadyExistException : Exception
    {
        public MembershipAlreadyExistException() : base($"This user is already a member of this course")
        {
        }
    }
}
