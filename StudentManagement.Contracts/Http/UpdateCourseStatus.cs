using StudentManagement.Contracts.Models;

namespace StudentManagement.Contracts.Http
{
    public class UpdateCourseStatusRequest
    {
        public CourseStatus Status { get; init; }
    }
}