using System;

namespace EduManagementLab.Core.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid userId) : base($"No user found with id {userId}")
        {
        }
    }
}
