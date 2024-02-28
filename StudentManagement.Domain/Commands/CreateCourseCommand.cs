using System.Threading;
using System.Threading.Tasks;

using MediatR;


using StudentManagement.Domain.Repositories;

using DatabaseCourse = StudentManagement.Domain.Models.Database.Course;


namespace StudnetManagement.Domain.Commands
{
    public class CreateCourseCommand : IRequest<CreateCourseResult>
    {
        public string Title { get; init; }
        public string Description { get; init; }
    }

    public class CreateCourseResult
    {
        public string Id { get; init; }
    }

    public class CreateCoureCommandHandler : IRequestHandler<CreateCourseCommand, CreateCourseResult>
    {
        private readonly IRepository _repository;

        public CreateCoureCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateCourseResult> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {

            DatabaseCourse course = await _repository.CreateCourse(request.Title, request.Description, cancellationToken);

            return new CreateCourseResult
            {
                Id = course.Id.ToString()
            };
        }
    }
}