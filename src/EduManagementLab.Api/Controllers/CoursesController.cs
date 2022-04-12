using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduManagementLab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            catch (CourseNotFoundException)
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
            return CreatedAtAction(nameof(GetCourse), new { courseId = course.Id }, _mapper.Map<CourseDto>(course));
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
        [Route("{courseId}/Memberships")]
        [ProducesResponseType(typeof(MembershipDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<MembershipDto> AddCourseMembership(MembershipDto membershipDto)
        {
            try
            {
                var membership = _courseService.CreateCourseMembership(membershipDto.CourseId, membershipDto.UserId, membershipDto.EnrolledDate);
                return CreatedAtAction(nameof(GetCourseMembership), new { courseId = membershipDto.CourseId, userId = membershipDto.UserId }, _mapper.Map<MembershipDto>(membership));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPut]
        [Route("{courseId}/Memberships/{userId}")]
        [ProducesResponseType(typeof(MembershipDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<MembershipDto> UpdateCourseMembershipEnrolledDate(MembershipDto membershipDto)
        {
            try
            {
                var membership = _courseService.UpdateMembershipEnrolledDate(membershipDto.CourseId, membershipDto.UserId, membershipDto.EnrolledDate);
                return Ok(_mapper.Map<MembershipDto>(membership));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<MembershipDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}/Memberships")]
        public ActionResult<List<MembershipDto>> GetCourseMemberships(Guid courseId)
        {
            try
            {
                var membershipList = _courseService.GetCourse(courseId, true).Memperships.ToList();
                var membershipDtoList = new List<MembershipDto>();

                foreach (var membership in membershipList)
                {
                    membershipDtoList.Add(_mapper.Map<MembershipDto>(membership));
                }

                return Ok(membershipDtoList);
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(MembershipDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}/Memberships/{userId}")]
        public ActionResult<MembershipDto> GetCourseMembership(Guid courseId, Guid userId)
        {
            try
            {
                var membershipList = _courseService.GetCourse(courseId, true).Memperships.ToList();
               
                if (membershipList.Any(u => u.UserId == userId))
                {
                    var selectedMembership = membershipList.FirstOrDefault(u => u.UserId == userId);

                    return Ok(_mapper.Map<MembershipDto>(selectedMembership));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(MembershipDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [Route("{courseId}/Memberships/{userId}")]
        public ActionResult<MembershipDto> DeleteCourseMembership(Guid courseId, Guid userId)
        {
            try
            {
                var membership = _courseService.RemoveCourseMembership(courseId, userId);
                return Ok(_mapper.Map<MembershipDto>(membership));
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
        public class MembershipDto
        {
            [Required]
            public Guid CourseId { get; set; }
            [Required]
            public Guid UserId { get; set; }
            [Required]
            public DateTime EnrolledDate { get; set; }
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

        public class CourseAutoMapperProfile : Profile
        {
            public CourseAutoMapperProfile()
            {
                CreateMap<Course, CourseDto>();
                CreateMap<Course.Membership, MembershipDto>();
            }
        }
    }
}
