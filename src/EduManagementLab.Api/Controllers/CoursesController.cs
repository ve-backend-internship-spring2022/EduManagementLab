using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduManagementLab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly UserService _userService;

        public CoursesController(CourseService courseService, UserService userService)
        {
            _courseService = courseService;
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Course>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Course>> GetCourses()
        {
            return Ok(_courseService.GetCourses().ToList());
        }


        [HttpGet]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}")]
        public ActionResult<Course> GetCourse(Guid id)
        {
            try
            {
                var course = _courseService.GetCourse(id);
                return Ok(course);
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }

        }


        [HttpPost]
        [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
        public ActionResult<Course> AddCourse(AddCourseModel addCourse)
        {
            var course = _courseService.CreateCourse(addCourse.Code, addCourse.Name, addCourse.Description, addCourse.StartDate, addCourse.EndDate);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }


        [HttpPatch()]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}/UpdateCourseInfo")]
        public ActionResult<Course> UpdateCourseInfo(UpdateCourseInfoModel updateCourseInfo)
        {
            try
            {
                var course = _courseService.UpdateCourseInfo(updateCourseInfo.Id, updateCourseInfo.Code, updateCourseInfo.Name, updateCourseInfo.Description);
                return Ok(course);
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }

        }


        [HttpPatch()]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}/UpdateCoursePeriod")]
        public ActionResult<Course> UpdateCoursePeriod(UpdateCoursePeriodModel updateCoursePeriod)
        {
            try
            {
                var course = _courseService.UpdateCoursePeriod(updateCoursePeriod.Id, updateCoursePeriod.StartDate, updateCoursePeriod.EndDate);
                return Ok(course);
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteCourse(Guid id)
        {
            try
            {
                _courseService.DeleteCourse(id);
                return Ok();
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpPost]
        [Route("{id}/AddCourseMembership")]
        public ActionResult<Course> AddCourseMembership(Guid id, Guid userId, DateTime enrolledDate)
        {
            try
            {
                var course = _courseService.CreateCourseMembership(id, userId, enrolledDate);
                return Ok(course);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpGet]
        [Route("{courseId}/GetCourseMemberships")]
        public ActionResult<IEnumerable<Course.Membership>> GetCourseMemberships(Guid courseId)
        {
            try
            {
                var courseMembership = _courseService.GetCourse(courseId).Memperships;
                return Ok(courseMembership);
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }


    }
    public class AddCourseModel
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
    public class UpdateCourseInfoModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

    }
    public class UpdateCoursePeriodModel
    {
        public Guid Id { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
    //public class MembershipModel
    //{
    //    public Guid Id { get; set; }
    //    public CourseModel? Course { get; set; }
    //    [Required]
    //    public Guid CourseId { get; set; }
    //    public UserModel? User { get; set; }
    //    [Required]
    //    public Guid UserId { get; set; }
    //    [Required]
    //    public DateTime EnrolledDate { get; set; }
    //}
    //public class UserModel
    //{
    //    [Required]
    //    public string Displayname { get; set; }
    //    [Required]
    //    public string FirstName { get; set; }
    //    [Required]
    //    public string LastName { get; set; }
    //    [Required]
    //    public string Email { get; set; }
    //}
    //public class CourseModel
    //{
    //    [Required]
    //    public string Code { get; set; }
    //    [Required]
    //    public string Name { get; set; }
    //    public string? Description { get; set; }
    //    [Required]
    //    public DateTime StartDate { get; set; }
    //    [Required]
    //    public DateTime EndDate { get; set; }
    //}
}
