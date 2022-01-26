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
        [ProducesResponseType(typeof(List<SimpleUserModel>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SimpleUserModel>> GetUsers()
        {
            try
            {
                var userlist = _userService.GetUsers().ToList();
                var simpleUserList = new List<SimpleUserModel>();

                foreach (var user in userlist)
                {
                    simpleUserList.Add(UserToSimpleUser(user));
                }

                return Ok(simpleUserList);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }     
        }

        [HttpGet]
        [ProducesResponseType(typeof(SimpleUserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{userId}")]
        public ActionResult<SimpleUserModel> GetUser(Guid userId)
        {
            try
            {
                var user = _userService.GetUser(userId);
                return Ok(UserToSimpleUser(user));
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SimpleUserModel), StatusCodes.Status201Created)]
        public ActionResult<SimpleUserModel> AddUser(SimpleUserModel addUser)
        {
            var user = _userService.CreateUser(addUser.DisplayName, addUser.FirstName, addUser.LastName, addUser.Email);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, UserToSimpleUser(user));
        }

        [HttpPost]
        [ProducesResponseType(typeof(SimpleUserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{userId}/UpdateName")]
        public ActionResult<SimpleUserModel> UpdateName(UpdateNameModel updateName)
        {
            try
            {
                var user = _userService.UpdateName(updateName.Id, updateName.DisplayName, updateName.FirstName, updateName.LastName);
                return Ok(UserToSimpleUser(user));
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SimpleUserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{userId}/UpdateEmail")]
        public ActionResult<SimpleUserModel> UpdateEmail(UpdateEmailModel updateEmail)
        {
            try
            {
                var user = _userService.UpdateEmail(updateEmail.Id, updateEmail.Email);
                return Ok(UserToSimpleUser(user));
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{userid}")]
        public ActionResult DeleteUser(Guid userId)
        {
            try
            {
                _userService.DeleteUser(userId);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        private SimpleUserModel UserToSimpleUser(User user)
        {
            var simpleUser = new SimpleUserModel()
            {
                DisplayName = user.Displayname,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return simpleUser;
        }

        public class SimpleUserModel
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
