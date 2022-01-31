﻿using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduManagementLab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public CoursesController(CourseService courseService, UserService userService, IMapper mapper)
        {
            _courseService = courseService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CourseDto>> GetCourses()
        {
            try
            {
                var Courselist = _courseService.GetCourses().ToList();
                var courseDtoList = new List<CourseDto>();

                foreach (var course in Courselist)
                {
                    courseDtoList.Add(_mapper.Map<CourseDto>(course));
                }

                return Ok(courseDtoList);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpGet]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}")]
        public ActionResult<CourseDto> GetCourse(Guid courseId)
        {
            try
            {
                var course = _courseService.GetCourse(courseId);
                return Ok(_mapper.Map<CourseDto>(course));
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }

        }


        [HttpPost]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
        public ActionResult<CourseDto> AddCourse(CreateCourseRequest createCourseRequest)
        {
            var course = _courseService.CreateCourse(createCourseRequest.Code, createCourseRequest.Name, createCourseRequest.Description, createCourseRequest.StartDate, createCourseRequest.EndDate);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, _mapper.Map<CourseDto>(course));
        }


        [HttpPatch()]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}/UpdateCourseInfo")]
        public ActionResult<CourseDto> UpdateCourseInfo(UpdateCourseInfoRequest updateCourseInfoRequest)
        {
            try
            {
                var course = _courseService.UpdateCourseInfo(updateCourseInfoRequest.Id, updateCourseInfoRequest.Code, updateCourseInfoRequest.Name, updateCourseInfoRequest.Description);
                return Ok(_mapper.Map<CourseDto>(course));
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }

        }


        [HttpPatch()]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}/UpdateCoursePeriod")]
        public ActionResult<CourseDto> UpdateCoursePeriod(UpdateCoursePeriodRequest updateCoursePeriodRequest)
        {
            try
            {
                var course = _courseService.UpdateCoursePeriod(updateCoursePeriodRequest.Id, updateCoursePeriodRequest.StartDate, updateCoursePeriodRequest.EndDate);
                return Ok(_mapper.Map<CourseDto>(course));
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpDelete("{courseId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteCourse(Guid courseId)
        {
            try
            {
                _courseService.DeleteCourse(courseId);
                return Ok();
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpPost()]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [Route("{courseId}/Membership")]
        public ActionResult<CourseDto> AddCourseMembership(Guid courseId, Guid userId, DateTime enrolledDate)
        {
            try
            {
                var course = _courseService.CreateCourseMembership(courseId, userId, enrolledDate);
                return Ok(_mapper.Map<CourseDto>(course));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [HttpGet]
        [Route("{courseId}/Memberships")]
        public ActionResult<List<Course.Membership>> GetCourseMemberships(Guid courseId)
        {
            try
            {
                var courseMembership = _courseService.GetCourse(courseId, true).Memperships.ToList();
                return Ok(courseMembership);
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpDelete]
        [Route("{courseId}/Membership")]
        public ActionResult<Course> DeleteCourseMembership(Guid courseId, Guid userId)
        {
            try
            {
                var courseMembership = _courseService.RemoveCourseMembership(courseId, userId);
                return Ok(courseMembership);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        public class CourseDto
        {
            public Guid Id { get; set; }
            [Required]
            public string Code { get; set; }
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; }
            [Required]
            public DateTime StartDate { get; set; }
            [Required]
            public DateTime EndDate { get; set; }
        }
        public class CreateCourseRequest
        {
            [Required]
            public string Code { get; set; }
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; }
            [Required]
            public DateTime StartDate { get; set; }
            [Required]
            public DateTime EndDate { get; set; }
        }
        public class UpdateCourseInfoRequest
        {
            public Guid Id { get; set; }
            [Required]
            public string Code { get; set; }
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; }

        }
        public class UpdateCoursePeriodRequest
        {
            public Guid Id { get; set; }
            [Required]
            public DateTime StartDate { get; set; }
            [Required]
            public DateTime EndDate { get; set; }
        }

        public class UserAutoMapperProfile : Profile
        {
            public UserAutoMapperProfile()
            {
                CreateMap<Course, CourseDto>();
            }
        }
    }
}
