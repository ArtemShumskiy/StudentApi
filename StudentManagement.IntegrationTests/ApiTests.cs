using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;

using Newtonsoft.Json;

using Shouldly;

using StudentManagement.Contracts.Http;

using ContractsCourse = StudentManagement.Contracts.Models.Course;
using ContractsCourseStatus = StudentManagement.Contracts.Models.CourseStatus;

namespace StudentManagement.IntegrationTests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateCourseShouldDoItSuccessfully()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            string title = Guid.NewGuid().ToString();
            string description = Guid.NewGuid().ToString();

            // Act
            HttpResponseMessage response = await client.PutAsync("courses", new StringContent(
                JsonConvert.SerializeObject(new CreateCourseRequest
                {
                    Title = title,
                    Description = description
                }), Encoding.UTF8, "application/json"));

            // Assert
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            CreateCourseResponse responseModel = JsonConvert.DeserializeObject<CreateCourseResponse>(responseString);

            responseModel.Id.ShouldNotBeEmpty();
            response.Headers.Location.ToString().ShouldBe($"courses/{responseModel.Id}");
        }

        [Fact]
        public async Task GetCourseByIdShouldDoItSuccessfully()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            string title = Guid.NewGuid().ToString();
            string description = Guid.NewGuid().ToString();

            HttpResponseMessage createResponse = await client.PutAsync("courses", new StringContent(
                JsonConvert.SerializeObject(new CreateCourseRequest
                {
                    Title = title,
                    Description = description
                }), Encoding.UTF8, "application/json"));

            _ = createResponse.EnsureSuccessStatusCode();
            string createResponseString = await createResponse.Content.ReadAsStringAsync();
            CreateCourseResponse createResponseModel = JsonConvert.DeserializeObject<CreateCourseResponse>(createResponseString);

            // Act
            HttpResponseMessage getResponse = await client.GetAsync($"courses/{createResponseModel.Id}");

            // Assert
            _ = getResponse.EnsureSuccessStatusCode();
            string getResponseString = await getResponse.Content.ReadAsStringAsync();
            GetCourseByIdResponse getResponseModel = JsonConvert.DeserializeObject<GetCourseByIdResponse>(getResponseString);
            ContractsCourse course = getResponseModel.Course;

            _ = course.ShouldNotBeNull();
            course.Id.ShouldBe(createResponseModel.Id);
            course.Title.ShouldBe(title);
            course.Description.ShouldBe(description);
            course.Status.ShouldBe(ContractsCourseStatus.New);
            course.AssigneeEmail.ShouldBeNull();
            course.UpdatedAt.ShouldBeNull();
        }

        [Fact]
        public async Task AssignToStudentShouldDoItSuccessfully()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            string title = Guid.NewGuid().ToString();
            string description = Guid.NewGuid().ToString();

            HttpResponseMessage createResponse = await client.PutAsync("courses", new StringContent(
                JsonConvert.SerializeObject(new CreateCourseRequest
                {
                    Title = title,
                    Description = description
                }), Encoding.UTF8, "application/json"));

            _ = createResponse.EnsureSuccessStatusCode();
            string createResponseString = await createResponse.Content.ReadAsStringAsync();
            CreateCourseResponse createResponseModel = JsonConvert.DeserializeObject<CreateCourseResponse>(createResponseString);

            AssignCourseRequest request = new()
            {
                Email = $"{Guid.NewGuid():N}@gmail.com"
            };

            // Act
            HttpResponseMessage assignResponse = await client.PostAsync($"courses/{createResponseModel.Id}", new StringContent(
                JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            // Assert
            _ = assignResponse.EnsureSuccessStatusCode();

            HttpResponseMessage getResponse = await client.GetAsync($"courses/{createResponseModel.Id}");

            _ = getResponse.EnsureSuccessStatusCode();
            string getResponseString = await getResponse.Content.ReadAsStringAsync();
            GetCourseByIdResponse getResponseModel = JsonConvert.DeserializeObject<GetCourseByIdResponse>(getResponseString);
            ContractsCourse course = getResponseModel.Course;
            course.AssigneeEmail.ShouldBe(request.Email);
            course.Status.ShouldBe(ContractsCourseStatus.Assigned);
        }
    }
}