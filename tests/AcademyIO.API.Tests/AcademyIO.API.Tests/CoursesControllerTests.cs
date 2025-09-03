using AcademyIO.Api.Tests.Config;
using AcademyIO.ManagementCourses.Application.Queries.ViewModels;
using System.Net.Http.Json;

namespace AcademyIO.API.Tests
{
    [TestCaseOrderer("AcademyIO.API.Tests.Config.PriorityOrderer", "AcademyIO.API.Tests")]
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class CoursesControllerTests
    {
        private readonly IntegrationTestsFixture _fixture;

        public CoursesControllerTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task AddCourse_Success()
        {
            // Arrange
            var course = new CourseViewModel
            {
                Name = "Lesson 1",
                Id = Guid.NewGuid(),
                Description = "Random",
                Price = 800
            };

            await _fixture.LoginApi();
            _fixture.Client.SetToken(_fixture.Token);

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"/api/courses/create", course);

            await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}