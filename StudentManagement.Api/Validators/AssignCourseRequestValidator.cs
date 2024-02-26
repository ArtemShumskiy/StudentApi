using FluentValidation;

using StudentManagement.Contracts.Http;

namespace StudentManagement.Api.Validators
{
    public class AssignCourseRequestValidator : AbstractValidator<AssignCourseRequest>
    {
        public AssignCourseRequestValidator()
        {
            _ = RuleFor(x => x.Email).EmailAddress().NotNull();
        }
    }
}