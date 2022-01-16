using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(_userService.GetUsers().ToList());
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<User> GetUser(Guid id)
        {
            try
            {
                var user = _userService.GetUser(id);
                return Ok(user);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult<User> AddUser(AddUserModel addUser)
        {
            var user = _userService.CreateUser(addUser.DisplayName, addUser.FirstName, addUser.LastName, addUser.Email);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPost]
        [Route("{id}/UpdateName")]
        public ActionResult<User> UpdateName(UpdateNameModel updateName)
        {
            try
            {
                var user = _userService.UpdateName(updateName.Id, updateName.DisplayName, updateName.FirstName, updateName.LastName);
                return Ok(user);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{id}/UpdateEmail")]
        public ActionResult<User> UpdateEmail(UpdateEmailModel updateEmail)
        {
            try
            {
                var user = _userService.UpdateEmail(updateEmail.Id, updateEmail.Email);
                return Ok(user);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteUser(Guid id)
        {
            try
            {
                _userService.DeleteUser(id);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        public class AddUserModel
        {
            [Required]
            public string DisplayName { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string Email { get; set; }
        }

        public class UpdateNameModel
        {
            [Required]
            public Guid Id { get; set; }
            [Required]
            public string DisplayName { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
        }

        public class UpdateEmailModel
        {
            [Required]
            public Guid Id { get; set; }
            [Required]
            public string Email { get; set; }
        }
    }
}
