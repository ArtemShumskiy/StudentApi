using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using StudentManagement.Contracts.Http;
using StudentManagement.Domain.Exceptions;

namespace StudentManagement.Api.Filters
{
    public class StudentManagementExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is StudentManagementException exception)
            {
                context.Result = exception.Error switch
                {
                    StudentManagementError.CourseNotFound => new NotFoundObjectResult(new ErrorModel
                    {
                        Message = exception.Message,
                    }),
                    StudentManagementError.CourseStatusIsNotNew => new BadRequestObjectResult(new ErrorModel
                    {
                        Message = exception.Message,
                    }),
                    StudentManagementError.CourseAlreadyAssignedToStudent => new BadRequestObjectResult(new ErrorModel
                    {
                        Message = exception.Message,
                    }),
                    StudentManagementError.CourseStatusCannotBeChanged => new BadRequestObjectResult(new ErrorModel
                    {
                        Message = exception.Message,
                    }),
                    _ => throw new ArgumentOutOfRangeException(nameof(context), exception.Error, null),
                };
            }
        }
    }
}