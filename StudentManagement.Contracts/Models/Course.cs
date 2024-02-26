using System;

namespace StudentManagement.Contracts.Models
{
    public class Course
    {
        public string Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public CourseStatus Status { get; init; }
        public string AssigneeEmail { get; init; }
    }
}