﻿using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace EduManagementLab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public UsersController(UserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            try
            {
                var userlist = _userService.GetUsers().ToList();
                var userDtoList = new List<UserDto>();

                foreach (var user in userlist)
                {
                    userDtoList.Add(_mapper.Map<UserDto>(user));
                }

                return Ok(userDtoList);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }     
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{userId}")]
        public ActionResult<UserDto> GetUser(Guid userId)
        {
            try
            {
                var user = _userService.GetUser(userId);
                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public ActionResult<UserDto> AddUser(CreateUserRequest createUserRequest)
        {
            var user = _userService.CreateUser(createUserRequest.DisplayName, createUserRequest.FirstName, createUserRequest.LastName, createUserRequest.Email);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, _mapper.Map<UserDto>(user));
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{userId}/UpdateName")]
        public ActionResult<UserDto> UpdateName(UpdateUserNameRequest updateUserNameRequest)
        {
            try
            {
                var user = _userService.UpdateName(updateUserNameRequest.Id, updateUserNameRequest.DisplayName, updateUserNameRequest.FirstName, updateUserNameRequest.LastName);
                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{userId}/UpdateEmail")]
        public ActionResult<UserDto> UpdateEmail(UpdateUserEmailRequest updateUserEmailRequest)
        {
            try
            {
                var user = _userService.UpdateEmail(updateUserEmailRequest.Id, updateUserEmailRequest.Email);
                return Ok(_mapper.Map<UserDto>(user));
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
        public class UserDto
        {
            public Guid Id { get; set; }
            [Required]
            public string DisplayName { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string Email { get; set; }
        }

        public class CreateUserRequest
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

        public class UpdateUserNameRequest
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

        public class UpdateUserEmailRequest
        {
            [Required]
            public Guid Id { get; set; }
            [Required]
            public string Email { get; set; }
        }

        public class CourseAutoMapperProfile : Profile
        {
            public CourseAutoMapperProfile()
            {
                CreateMap<User, UserDto>();
            }
        }


    }
}
