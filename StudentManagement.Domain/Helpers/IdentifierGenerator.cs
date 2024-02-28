using System;

namespace StudentManagement.Domain.Helpers
{
    public interface IIdentifierGenerator
    {
        Guid Generate();
    }

    public class IdentifierGenerator : IIdentifierGenerator
    {
        public Guid Generate()
        {
            return Guid.NewGuid();
        }
    }
}