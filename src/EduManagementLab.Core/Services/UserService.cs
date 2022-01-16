using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<User> GetUsers()
        {
            return _unitOfWork.Users.GetAll();
        }

        public User CreateUser(string displayname, string firstName, string lastName, string email)
        {
            var user = new User() { Displayname = displayname, FirstName = firstName, LastName = lastName, Email = email };
            _unitOfWork.Users.Add(user);
            _unitOfWork.Complete();
            return user;
        }

        public User GetUser(Guid id)
        {
            var user = _unitOfWork.Users.GetById(id);
            if (user == null)
            {
                throw new UserNotFoundException(id);
            }
            return user;
        }

        public User UpdateName(Guid id, string displayName, string firstName, string lastName)
        {
            var user = GetUser(id);
            user.Displayname = displayName;
            user.FirstName = firstName;
            user.LastName = lastName;
            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();
            return user;
        }

        public User UpdateEmail(Guid id, string email)
        {
            var user = GetUser(id);
            user.Email = email;
            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();
            return user;
        }

        public void DeleteUser(Guid id)
        {
            var user = GetUser(id);
            _unitOfWork.Users.Remove(user);
            _unitOfWork.Complete();
        }
    }
}
