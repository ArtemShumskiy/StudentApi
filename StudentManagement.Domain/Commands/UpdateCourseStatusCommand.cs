using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using StudentManagement.Domain.Exceptions;
using StudentManagement.Domain.Repositories;

using DatabaseCourse = StudentManagement.Domain.Models.Database.Course;
using ContractsCourseStatus = StudentManagement.Contracts.Models.CourseStatus;

namespace StudentManagement.Domain.Commands
{
    public class UpdateCourseStatusCommand : IRequest<UpdateCourseStatusResult>
    {
        public string CourseId { get; init; }
        public ContractsCourseStatus Status { get; init; }
    }

    public class UpdateCourseStatusResult
    {
    }

    internal class UpdateCourseStatusCommandHandler
    {
        private static readonly IDictionary<ContractsCourseStatus, ContractsCourseStatus[]> AllowedStatuses =
            new ReadOnlyDictionary<ContractsCourseStatus, ContractsCourseStatus[]>(
                new Dictionary<ContractsCourseStatus, ContractsCourseStatus[]>
                {
                    { ContractsCourseStatus.New, new[] { ContractsCourseStatus.Cancelled } },
                    { ContractsCourseStatus.Assigned, new[] { ContractsCourseStatus.InProgress, ContractsCourseStatus.Cancelled } },
                    { ContractsCourseStatus.InProgress, new[] { ContractsCourseStatus.Completed, ContractsCourseStatus.Cancelled } },
                }
            );

        private readonly IRepository _repository;

        public UpdateCourseStatusCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        protected async Task<UpdateCourseStatusResult> HandleInternal(UpdateCourseStatusCommand request, CancellationToken cancellationToken)
        {
            DatabaseCourse course = await _repository.GetCourseById(request.CourseId, cancellationToken);
            if (course is null)
            {
                throw new StudentManagementException(
                    StudentManagementError.CourseNotFound,
                    $"Course with id {request.CourseId} not found"
                );
            }

            if (!AllowedStatuses.TryGetValue((ContractsCourseStatus)course.Status, out ContractsCourseStatus[] allowedStatuses))
            {
                throw new StudentManagementException(
                    StudentManagementError.CourseStatusCannotBeChanged,
                    $"Course status cannot be changed from {(ContractsCourseStatus)course.Status}"
                );
            }

            if (!Array.Exists(allowedStatuses, status => status == request.Status))
            {
                throw new StudentManagementException(
                    StudentManagementError.CourseStatusCannotBeChanged,
                    $"Course status cannot be changed from {(ContractsCourseStatus)course.Status} to {request.Status}"
                );
            }

            await _repository.UpdateCourseStatus(request.CourseId, request.Status, cancellationToken);
            return new();
        }
    }
}