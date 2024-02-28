using System.Threading;
using System.Threading.Tasks;

using StudentManagement.Domain.Exceptions;
using MediatR;

using DatabaseCourse = StudentManagement.Domain.Models.Database.Course;
using DatabaseStudent = StudentManagement.Domain.Models.Database.Student;
using ContractsCourseStatus = StudentManagement.Contracts.Models.CourseStatus;

using StudentManagement.Domain.Repositories;

namespace StudentManagement.Domain.Commands
{
    public class AssignCourseToStudentCommand : IRequest<AssignCourseToStudentCommandResult>
    {
        public string CourseId { get; init; }
        public string Email { get; init; }
    }

    public class AssignCourseToStudentCommandResult
    {
    }

    public class AssignCourseToStudentCommandHandler 
    {
        private readonly IRepository _repository;

        public AssignCourseToStudentCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public AssignCourseToStudentCommandHandler(IRepository repository, AssignCourseToStudentCommandHandler assignCourseToStudentCommandHandler) : this(repository)
        {
        }

        //override
        protected  async Task<AssignCourseToStudentCommandResult> HandleInternal(AssignCourseToStudentCommand request, CancellationToken cancellationToken)
        {
            DatabaseCourse course = await _repository.GetCourseById(request.CourseId, cancellationToken);
            if (course is null)
            {
                throw new StudentManagementException(
                    StudentManagementError.CourseNotFound,
                    $"Course with id {request.CourseId} not found"
                );
            }
            if (course.Status != (int)ContractsCourseStatus.New)
            {
                throw new StudentManagementException(
                    StudentManagementError.CourseStatusIsNotNew,
                    $"Course status is not {ContractsCourseStatus.New} but {(ContractsCourseStatus)course.Status}"
                );
            }

            if (course.AssigneeId is not null)
            {
                throw new StudentManagementException(
                    StudentManagementError.CourseAlreadyAssignedToStudent,
                    $"Task is already assigned to student with id {course.AssigneeId}"
                );
            }

            DatabaseStudent student = await _repository.CreateStudentIfNotExists(request.Email, cancellationToken);
            await _repository.AssignToStudent(course.Id.ToString(), student.Id, cancellationToken);

            return new();
        }
    }
}