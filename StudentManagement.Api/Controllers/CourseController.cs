using System.Threading;
using System.Threading.Tasks;

using MediatR;
using StudentManagement.Api.Filters;

using Microsoft.AspNetCore.Mvc;

using StudentManagement.Domain.Queries;
using StudentManagement.Contracts.Http;
using StudnetManagement.Domain.Commands;
using StudentManagement.Domain.Commands;

namespace StudentManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [StudentManagementExceptionFilter]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Create new Course
        /// </summary>
        /// <param name="request"Course information</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Identifier of newly created course</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///    PUT /Courses
        ///    {
        ///        "title": "Course title",
        ///        "description": "Course description"
        ///    }   
        /// 
        /// </remarks> 
        /// <response code="201">Returns newly created course identifier</response>
        /// <response code="400">If request is invalid</response>
        /// <response code="500">If something went wrong</response>
        [HttpPut]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(CreateCourseResponse), 201)]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request,
            CancellationToken cancellationToken = default)
        {
            CreateCourseCommand command = new()
            {
                Title = request.Title,
                Description = request.Description
            };

            CreateCourseResult result = await _mediator.Send(command, cancellationToken);

            return Created($"courses/{result.Id}", new CreateCourseResponse
            {
                Id = result.Id
            });
        }

        /// <summary>
        /// Get course by id
        /// </summary>
        /// <param name="id">Course identifier</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Course information</returns>
        /// <response code="200">Returns course information</response>
        /// <response code="404">If course with given id not found</response>
        /// <response code="500">If something went wrong</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetCourseByIdResponse), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<IActionResult> GetCourseById(string id, CancellationToken cancellationToken = default)
        {
            GetCourseByIdQuery query = new()
            {
                Id = id
            };

            GetCourseByIdResult result = await _mediator.Send(query, cancellationToken);

            return result.Course == null
                ? NotFound(new ErrorModel
                {
                    Message = $"Course with id {id} not found"
                })
                : Ok(new GetCourseByIdResponse
                {
                    Course = result.Course
                });
        }


        /// <summary>
        /// Assign course to student
        /// </summary>
        /// <param name="courseId">Course identifier</param>
        /// <param name="request">Assign information</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Nothing</returns>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///   POST /coursess/{courseId}
        ///   {
        ///     "email": "student@example"
        ///   }
        /// 
        /// </remarks>
        /// <response code="204">If course assigned successfully</response>
        /// <response code="400">If request is invalid</response>
        /// <response code="404">If course with given id not found</response>
        /// <response code="500">If something went wrong</response>
        [HttpPost("{courseId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<IActionResult> AssignToStudent([FromRoute] string courseId,
            [FromBody] AssignCourseRequest request,
            CancellationToken cancellationToken = default)
        {
            AssignCourseToStudentCommand command = new()
            {
                CourseId = courseId,
                Email = request.Email
            };

            _ = await _mediator.Send(command, cancellationToken);
            return NoContent();
        }


        /// <summary>
        /// Update course status
        /// </summary>
        /// <param name="courseId">Course identifier</param>
        /// <param name="request">Update information</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Nothing</returns>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        ///  PATCH /courses/{courseId}/status
        ///  {
        ///     "status": "InProgress"
        ///  }
        /// 
        /// </remarks>
        /// <response code="204">If course status updated successfully</response>
        /// <response code="404">If course with given id not found</response>
        /// <response code="500">If something went wrong</response>
        [HttpPatch("{courseId}/status")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<IActionResult> UpdateCourseStatus([FromRoute] string courseId,
            [FromBody] UpdateCourseStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            UpdateCourseStatusCommand command = new()
            {
                CourseId = courseId,
                Status = request.Status
            };

            _ = await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

    }
}