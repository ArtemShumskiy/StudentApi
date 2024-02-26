using FluentValidation;

using StudentManagement.Contracts.Http;

namespace StudentManagement.Api.Validators
{
    public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
    {
        public CreateCourseRequestValidator()
        {
            _ = RuleFor(x => x.Title).Length(1, 255).NotNull();
            _ = RuleFor(x => x.Description).NotEmpty().NotNull();
        }
    }
}