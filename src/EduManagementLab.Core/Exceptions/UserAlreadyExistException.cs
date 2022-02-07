using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Exceptions
{
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException(string email) : base($"User email {email} already exist")
        {
        }
    }
}
