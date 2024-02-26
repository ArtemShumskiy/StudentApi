using StudentManagement.Contracts.Models;

namespace StudentManagement.Contracts.Http
{
    public class GetCourseByIdResponse
    {
        public Course Course { get; init; }
    }
}