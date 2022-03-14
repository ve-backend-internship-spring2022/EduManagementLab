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


        //TODO: Validate if username and password are right
        public bool ValidateCredentials(string userName, string password)
        {
            var user = GetUserUsername(userName);
            if (user != null)
            {
                return user.Password.Equals(password);
            }

            return false;
        }

        //TODO: find username and return Username object 
        public User GetUserUsername(string userName)
        {
            var users = GetUsers();
            return users.FirstOrDefault(x => x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public User CreateUser(string displayname, string firstName, string lastName, string email)
        {
            var user = new User() { UserName = displayname, FirstName = firstName, LastName = lastName, Email = email };
            var allUsers = GetUsers();
            if (allUsers.Any(x => x.Email == email))
            {
                throw new UserAlreadyExistException(email);
            }
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
