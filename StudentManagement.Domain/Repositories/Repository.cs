using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using StudentManagement.Contracts.Models;
using StudentManagement.Domain.DbContexts;
using StudentManagement.Domain.Helpers;
using StudentManagement.Domain.Exceptions;

using DatabaseCourse = StudentManagement.Domain.Models.Database.Course;
using DatabaseStudent = StudentManagement.Domain.Models.Database.Student;
using ContractsCourseStatus = StudentManagement.Contracts.Models.CourseStatus;

namespace StudentManagement.Domain.Repositories
{
    public interface IRepository
    {
        Task<DatabaseCourse> CreateCourse(string title, string description, CancellationToken cancellationToken = default);
        Task<DatabaseCourse> GetCourseById(string id, CancellationToken cancellationToken = default);
        Task<DatabaseStudent> CreateStudentIfNotExists(string email, CancellationToken cancellationToken = default);
        Task AssignToStudent(string courseId, long studentId, CancellationToken cancellationToken = default);
        Task UpdateCourseStatus(string courseId, ContractsCourseStatus status, CancellationToken cancellationToken = default);
    }

    public class Repository : IRepository
    {
        private readonly StudentManagementContext _dbContext;
         private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIdentifierGenerator _identifierGenerator;

        public Repository(StudentManagementContext dbContext,
                        IDateTimeProvider dateTimeProvider,
                        IIdentifierGenerator identifierGenerator)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
            _identifierGenerator = identifierGenerator;
        }

        public async Task<DatabaseCourse> CreateCourse(string title, string description,
            CancellationToken cancellationToken = default)
        {
            DatabaseCourse course = new()
            {
                Title = title,
                Description = description,
                Id = _identifierGenerator.Generate(),
                CreatedAt = _dateTimeProvider.Now,
            };
            _ = await _dbContext.Courses.AddAsync(course, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);

            return course;
        }

        public async Task<DatabaseStudent> CreateStudentIfNotExists(string email, CancellationToken cancellationToken = default)
        {
            DatabaseStudent student = await _dbContext.Students.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (student is not null)
            {
                return student;
            }

            student = new DatabaseStudent
            {
                Email = email,
            };

            _ = await _dbContext.Students.AddAsync(student, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);

            return student;
        }

        public async Task AssignToStudent(string courseId, long studentId, CancellationToken cancellationToken = default)
        {
            DatabaseCourse course = await _dbContext.Courses.SingleAsync(x => x.Id.ToString() == courseId, cancellationToken);

            course.AssigneeId = studentId;
            course.UpdatedAt = _dateTimeProvider.Now;
            course.Status = (int)ContractsCourseStatus.Assigned;

            _ = await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<DatabaseCourse> GetCourseById(string id, CancellationToken cancellationToken = default)
        {
            return _dbContext.Courses
                .Include(t => t.Assignee)
                .SingleOrDefaultAsync(t => t.Id.ToString() == id, cancellationToken);
        }
        public async Task UpdateCourseStatus(string courseId, ContractsCourseStatus status, CancellationToken cancellationToken = default)
        {
            DatabaseCourse course = await _dbContext.Courses.SingleAsync(x => x.Id.ToString() == courseId, cancellationToken);

            course.Status = (int)status;
            course.UpdatedAt = _dateTimeProvider.Now;

            _ = await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}