using System;

namespace StudentManagement.Domain.Exceptions
{
    public enum StudentManagementError
    {
        CourseNotFound,
        CourseStatusIsNotNew,
        CourseAlreadyAssignedToStudent,
        CourseStatusCannotBeChanged,
    }

    public class StudentManagementException : Exception
    {
        public StudentManagementError Error { get; }

        public StudentManagementException(StudentManagementError error, string message) : base(message)
        {
            Error = error;
        }
    }
}