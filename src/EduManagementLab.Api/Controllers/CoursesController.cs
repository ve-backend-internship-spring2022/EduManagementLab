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
        [ProducesResponseType(typeof(List<SimpleCourseModel>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SimpleCourseModel>> GetCourses()
        {
            try
            {
                var Courselist = _courseService.GetCourses().ToList();
                var simpleCourseList = new List<SimpleCourseModel>();

                foreach (var course in Courselist)
                {
                    simpleCourseList.Add(CourseToSimpleCourse(course));
                }

                return Ok(simpleCourseList);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpGet]
        [ProducesResponseType(typeof(SimpleCourseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}")]
        public ActionResult<SimpleCourseModel> GetCourse(Guid courseId)
        {
            try
            {
                var course = _courseService.GetCourse(courseId);
                return Ok(CourseToSimpleCourse(course));
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }

        }


        [HttpPost]
        [ProducesResponseType(typeof(SimpleCourseModel), StatusCodes.Status201Created)]
        public ActionResult<SimpleCourseModel> AddCourse(SimpleCourseModel addCourse)
        {
            var course = _courseService.CreateCourse(addCourse.Code, addCourse.Name, addCourse.Description, addCourse.StartDate, addCourse.EndDate);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, CourseToSimpleCourse(course));
        }


        [HttpPatch()]
        [ProducesResponseType(typeof(SimpleCourseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}/UpdateCourseInfo")]
        public ActionResult<SimpleCourseModel> UpdateCourseInfo(UpdateCourseInfoModel updateCourseInfo)
        {
            try
            {
                var course = _courseService.UpdateCourseInfo(updateCourseInfo.Id, updateCourseInfo.Code, updateCourseInfo.Name, updateCourseInfo.Description);
                return Ok(CourseToSimpleCourse(course));
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }

        }


        [HttpPatch()]
        [ProducesResponseType(typeof(SimpleCourseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}/UpdateCoursePeriod")]
        public ActionResult<SimpleCourseModel> UpdateCoursePeriod(UpdateCoursePeriodModel updateCoursePeriod)
        {
            try
            {
                var course = _courseService.UpdateCoursePeriod(updateCoursePeriod.Id, updateCoursePeriod.StartDate, updateCoursePeriod.EndDate);
                return Ok(CourseToSimpleCourse(course));
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpDelete("{courseId}")]
        [ProducesResponseType(typeof(SimpleCourseModel), StatusCodes.Status200OK)]
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
        [Route("{courseId}/Membership")]
        public ActionResult<Course> AddCourseMembership(Guid courseId, Guid userId, DateTime enrolledDate)
        {
            try
            {
                var course = _courseService.CreateCourseMembership(courseId, userId, enrolledDate);
                return Ok(course);
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
        private SimpleCourseModel CourseToSimpleCourse(Course course)
        {
            var simpleCourse = new SimpleCourseModel()
            {
                Code = course.Code,
                Name = course.Name,
                Description = course.Description,
                StartDate = course.StartDate,
                EndDate = course.EndDate
            };

            return simpleCourse;
        }
        public class SimpleCourseModel
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
    }
}
