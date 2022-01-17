using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduManagementLab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CoursesController(CourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [Route("GetCourses")]
        public ActionResult<IEnumerable<Course>> GetCourses()
        {
            return Ok(_courseService.GetCourses().ToList());
        }


        [HttpGet]
        [Route("GetCourse/{id}")]
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
        [Route("AddCourse")]
        public ActionResult<Course> AddCourse(AddCourseModel addCourse)
        {
            var course = _courseService.CreateCourse(addCourse.Code, addCourse.Name, addCourse.Description, addCourse.StartDate, addCourse.EndDate);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }


        [HttpPut()]
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
        [HttpPut()]
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
}
