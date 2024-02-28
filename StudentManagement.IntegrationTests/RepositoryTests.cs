using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Moq;

using Shouldly;

using StudentManagement.Domain.DbContexts;
using StudentManagement.Domain.Helpers;
using StudentManagement.Domain.Repositories;

using DatabaseCourse = StudentManagement.Domain.Models.Database.Course;
using DatabaseStudent = StudentManagement.Domain.Models.Database.Student;
using ContractsCourseStatus = StudentManagement.Contracts.Models.CourseStatus;

namespace StudentManagement.IntegrationTests
{
    public class RepositoryTests
    {
        private readonly IRepository _repository;

        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
        private readonly Mock<IIdentifierGenerator> _identifierGeneratorMock = new();

        public RepositoryTests()
        {
            _repository = new Repository(
                CreateContext(),
                _dateTimeProviderMock.Object,
                _identifierGeneratorMock.Object
            );
        }

        [Fact]
        public async Task CreateCourseShouldDoItSuccessfully()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            DateTime now = new(2021, 1, 1, 2, 3, 4, DateTimeKind.Utc);

            string title = Guid.NewGuid().ToString();
            string description = Guid.NewGuid().ToString();

            _ = _dateTimeProviderMock.SetupGet(m => m.Now).Returns(now);
            _ = _identifierGeneratorMock.Setup(m => m.Generate()).Returns(id);

            // Act
            DatabaseCourse result = await _repository.CreateCourse(title, description);

            await using StudentManagementContext testContext = CreateContext();
            bool courseExist = await testContext.Courses.AnyAsync(x => x.Id == id);

            // Assert
            courseExist.ShouldBeTrue();

            result.Id.ShouldBe(id);
            result.CreatedAt.ShouldBe(now);
            result.Title.ShouldBe(title);
            result.Description.ShouldBe(description);
            result.AssigneeId.ShouldBeNull();
            result.Assignee.ShouldBeNull();
            result.Status.ShouldBe((int)ContractsCourseStatus.New);
            result.UpdatedAt.ShouldBeNull();
            result.CreatedAt.ShouldBe(now);
        }

        [Fact]
        public async Task GetCourseByIdShouldReturnCourseWithoutAssignee()
        {
            // Arrange
            DatabaseCourse course = new()
            {
                Id = Guid.NewGuid(),
                CreatedAt = new DateTime(2021, 1, 1, 2, 3, 4, DateTimeKind.Utc),
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Status = 3,
                UpdatedAt = new DateTime(2022, 2, 3, 4, 5, 6, DateTimeKind.Utc),
            };

            await using StudentManagementContext testContext = CreateContext();
            _ = await testContext.Courses.AddAsync(course);
            _ = await testContext.SaveChangesAsync();

            // Act
            DatabaseCourse result = await _repository.GetCourseById(course.Id.ToString());

            // Assert
            _ = result.ShouldNotBeNull();
            result.Id.ShouldBe(course.Id);
            result.CreatedAt.ShouldBe(course.CreatedAt);
            result.Title.ShouldBe(course.Title);
            result.Description.ShouldBe(course.Description);
            result.AssigneeId.ShouldBeNull();
            result.Assignee.ShouldBeNull();
            result.Status.ShouldBe(course.Status);
            result.UpdatedAt.ShouldBe(course.UpdatedAt);
        }

        [Fact]
        public async Task GetCourseByIdShouldReturnNull()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            // Act
            DatabaseCourse result = await _repository.GetCourseById(id.ToString());

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task CreateStudentIfNotExistsShouldReturnStudent()
        {
            // Arrange
            DatabaseStudent student = new()
            {
                Email = $"{Guid.NewGuid():N}@gmail.com",
            };

            await using StudentManagementContext testContext = CreateContext();
            _ = await testContext.Students.AddAsync(student);
            _ = await testContext.SaveChangesAsync();

            // Act
            DatabaseStudent result = await _repository.CreateStudentIfNotExists(student.Email);

            // Assert
            _ = result.ShouldNotBeNull();
            result.Id.ShouldBe(student.Id);
        }

        [Fact]
        public async Task AssignToStudentShouldDoItSuccessfully()
        {
            // Arrange
            DatabaseCourse course = new()
            {
                Id = Guid.NewGuid(),
                CreatedAt = new DateTime(2021, 1, 1, 2, 3, 4, DateTimeKind.Utc),
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                UpdatedAt = new DateTime(2022, 2, 3, 4, 5, 6, DateTimeKind.Utc),
            };

            DatabaseStudent student = new()
            {
                Email = $"{Guid.NewGuid():N}@gmail.com"
            };

            DateTime now = new(2021, 1, 1, 2, 3, 4, DateTimeKind.Utc);
            _ = _dateTimeProviderMock.SetupGet(x => x.Now).Returns(now);

            await using StudentManagementContext testContext = CreateContext();
            _ = await testContext.Courses.AddAsync(course);
            _ = await testContext.Students.AddAsync(student);
            _ = await testContext.SaveChangesAsync();

            // Act
            await _repository.AssignToStudent(course.Id.ToString(), student.Id);

            await using StudentManagementContext testContext2 = CreateContext();
            DatabaseCourse result = await testContext2.Courses
                .Include(x => x.Assignee)
                .FirstOrDefaultAsync(x => x.Id == course.Id);

            // Assert
            _ = result.ShouldNotBeNull();
            result.AssigneeId.ShouldBe(student.Id);
            result.Assignee.Id.ShouldBe(student.Id);
            result.Assignee.Email.ShouldBe(student.Email);
            result.Status.ShouldBe((int)ContractsCourseStatus.Assigned);
            result.UpdatedAt.ShouldBe(now);
        }

        private static StudentManagementContext CreateContext()
        {
            DbContextOptions<StudentManagementContext> options = new DbContextOptionsBuilder<StudentManagementContext>()
                .UseNpgsql("Host=localhost;Database=task_management;Username=user;Password=password")
                .Options;

            return new StudentManagementContext(options);
        }
    }
}