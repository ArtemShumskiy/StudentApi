using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using MediatR;

using StudentManagement.Domain.Repositories;

using DatabaseCourse = StudentManagement.Domain.Models.Database.Course;
using ContractsCourse = StudentManagement.Contracts.Models.Course;

namespace StudentManagement.Domain.Queries
{
    public class GetCourseByIdQuery : IRequest<GetCourseByIdResult>
    {
        public string Id { get; init; }
    }

    public class GetCourseByIdResult
    {
        public ContractsCourse Course { get; init; }
    }

    internal class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, GetCourseByIdResult>
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public GetCourseByIdQueryHandler(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GetCourseByIdResult> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            DatabaseCourse dbResult = await _repository.GetCourseById(request.Id, cancellationToken);

            return new GetCourseByIdResult
            {
                Course = _mapper.Map<ContractsCourse>(dbResult)
            };
        }
    }
}