namespace StudentManagement.Contracts.Http
{
    public class CreateCourseRequest
    {
        public string Title { get; init; }
        public string Description { get; init; }
    }

    public class CreateCourseResponse
    {
        public string Id { get; init; }
    }
}