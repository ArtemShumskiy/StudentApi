using System;

namespace StudentManagement.Domain.Helpers
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}