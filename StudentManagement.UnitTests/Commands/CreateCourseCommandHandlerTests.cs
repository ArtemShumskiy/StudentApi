using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Moq;

using Shouldly;


using StudentManagement.Domain.Commands;
using StudentManagement.Domain.Repositories;

using StudnetManagement.Domain.Commands;

using DatabaseCourse = StudentManagement.Domain.Models.Database.Course;
namespace StudentManagement.UnitTests.Commands
{
    public class CreateCourseCommandTests
    {
        private readonly Mock<IRepository> _repositoryMock = new();
        private readonly IRequestHandler<CreateCourseCommand, CreateCourseResult> _handler;

        public CreateCourseCommandTests()
        {
             _handler = new CreateCoureCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task HandleShouldAddCourseToRepository()
        {
            // Arrange
            string title = Guid.NewGuid().ToString();
            string description = Guid.NewGuid().ToString();
            CreateCourseCommand command = new()
            {
                Title = title,
                Description = description
            };
            Guid id = Guid.NewGuid();

             _ = _repositoryMock.Setup(x => x.CreateCourse(title, description, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new DatabaseCourse
                {
                    Id = id
                }));

            // Act

            CreateCourseResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            result.Id.ShouldBe(id.ToString());
        }
    }
}