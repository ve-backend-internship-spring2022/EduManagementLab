using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace EduManagementLab.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CourseTasksController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly CourseTaskService _courseTaskService;
        private readonly IMapper _mapper;

        public CourseTasksController(CourseTaskService courseTaskService, CourseService courseService, IMapper mapper)
        {
            _courseTaskService = courseTaskService;
            _courseService = courseService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CourseTaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}/Coursetasks")]
        public ActionResult<IEnumerable<CourseTaskDto>> GetCourseTasks(Guid courseId)
        {
            try
            {
                var courseTaskList = _courseService.GetCourse(courseId).CourseTasks.ToList();
                var courseTaskDtoList = new List<CourseTaskDto>();

                foreach (var courseTask in courseTaskList)
                {
                    courseTaskDtoList.Add(_mapper.Map<CourseTaskDto>(courseTask));
                }

                return Ok(courseTaskDtoList);
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(CourseTaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseTaskId}")]
        public ActionResult<CourseTaskDto> GetCourseTask(Guid courseTaskId)
        {
            try
            {
                var courseTask = _courseTaskService.GetCourseTask(courseTaskId);
                return Ok(_mapper.Map<CourseTaskDto>(courseTask));
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CourseTaskDto), StatusCodes.Status201Created)]
        public ActionResult<CourseTaskDto> AddCourseTask(Guid courseId, string courseTaskName, string courseTaskDescription)
        {
            var newCourseTask = _courseTaskService.CreateCourseTask(courseId, courseTaskName, courseTaskDescription);
            return CreatedAtAction(nameof(GetCourseTask), new { courseTaskId = newCourseTask.Id }, _mapper.Map<CourseTaskDto>(newCourseTask));
        }

        [HttpPatch()]
        [ProducesResponseType(typeof(CourseTaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseTaskId}/UpdateCourseTaskInfo")]
        public ActionResult<CourseTaskDto> UpdateCourseTaskInfo(Guid courseTaskId, string courseTaskName, string courseTaskDescription)
        {
            try
            {
                var newCourseTask = _courseTaskService.UpdateCourseTask(courseTaskId, courseTaskName, courseTaskDescription);
                return Ok(_mapper.Map<CourseTaskDto>(newCourseTask));
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseTaskId}")]
        public ActionResult<CourseTaskDto> DeleteCourseTask(Guid courseTaskId)
        {
            try
            {
                var courseTask = _courseTaskService.DeleteCourseTask(courseTaskId);
                return Ok(_mapper.Map<CourseTaskDto>(courseTask));
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CourseTaskDto.ResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseTaskId}/Results")]
        public ActionResult<IEnumerable<CourseTaskDto.ResultDto>> GetCourseTaskResults(Guid courseTaskId)
        {
            try
            {
                var resultsList = _courseTaskService.GetCourseTask(courseTaskId, true).Results.ToList();
                var courseTaskResultsDtoList = new List<CourseTaskDto.ResultDto>();

                foreach (var result in resultsList)
                {
                    courseTaskResultsDtoList.Add(_mapper.Map<CourseTaskDto.ResultDto>(result));
                }

                return Ok(courseTaskResultsDtoList);
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CourseTaskDto.ResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseTaskId}/Results/{userId}")]
        public ActionResult<CourseTaskDto.ResultDto> GetCourseTaskResult(Guid courseTaskId, Guid memberId)
        {
            try
            {
                var resultList = _courseTaskService.GetCourseTask(courseTaskId).Results.ToList();

                if (resultList.Any(u => u.MembershipId == memberId))
                {
                    var selectedMembership = resultList.FirstOrDefault(u => u.MembershipId == memberId);

                    return Ok(_mapper.Map<CourseTaskDto.ResultDto>(selectedMembership));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{courseTaskId}/Results")]
        [ProducesResponseType(typeof(CourseTaskDto.ResultDto), StatusCodes.Status201Created)]
        public ActionResult<CourseTaskDto.ResultDto> AddCourseTaskResult(Guid courseTaskId, Guid memberId, decimal score)
        {
            var newCourseTaskResult = _courseTaskService.CreateCourseTaskResult(courseTaskId, memberId, score);
            return CreatedAtAction(nameof(GetCourseTaskResult), new { courseTaskId = newCourseTaskResult.CourseTaskId, memberId = newCourseTaskResult.MembershipId }, _mapper.Map<CourseTaskDto.ResultDto>(newCourseTaskResult));
        }

        [HttpPut]
        [ProducesResponseType(typeof(CourseTaskDto.ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseTaskId}/Results")]
        public ActionResult<CourseTaskDto.ResultDto> UpdateCourseTaskResult(Guid courseTaskId, Guid memberId, decimal score)
        {
            try
            {
                var newCourseTaskResult = _courseTaskService.UpdateCourseTaskResult(courseTaskId, memberId, score);
                return Ok(_mapper.Map<CourseTaskDto.ResultDto>(newCourseTaskResult));
            }
            catch (CourseTaskNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(CourseTaskDto.ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseTaskId}/Results/{membershipId}")]
        public ActionResult<CourseTaskDto.ResultDto> DeleteCourseTaskResult(Guid courseTaskId, Guid membershipId)
        {
            try
            {
                var result = _courseTaskService.DeleteCourseTaskResult(courseTaskId, membershipId);
                return Ok(_mapper.Map<CourseTaskDto.ResultDto>(result));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
        public class CourseTaskDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public DateTime LastUpdate { get; set; }
            public List<ResultDto> Results { get; set; } = new List<ResultDto>();
            public class ResultDto
            {
                public Guid Id { get; set; }
                public Course.Membership? Membership { get; set; }
                [Required]
                public Guid MembershipId { get; set; }
                [Required]
                public Guid CourseTaskId { get; set; }
                public decimal Score { get; set; }
                public DateTime LastUpdated { get; set; }
            }
        }

        public class CreateCourseTaskRequest
        {
            public Guid CourseId { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public DateTime LastUpdate { get; set; }
        }
        public class UpdateCourseTaskInfoRequest
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public DateTime LastUpdated { get; set; }
        }
        public class CourseTaskAutoMapperProfile : Profile
        {
            public CourseTaskAutoMapperProfile()
            {
                CreateMap<CourseTask, CourseTaskDto>();
                CreateMap<CourseTask.Result, CourseTaskDto.ResultDto>();
            }
        }
    }
}
