using AcademyIO.Api.Tests.Config;
using AcademyIO.ManagementCourses.Application.Queries.ViewModels;
using System.Net;
using System.Net.Http.Json;

namespace AcademyIO.API.Tests
{
    [TestCaseOrderer("AcademyIO.API.Tests.Config.PriorityOrderer", "AcademyIO.API.Tests")]
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class LessonsControllerTests : IClassFixture<IntegrationTestsFixture>, IAsyncLifetime
    {
        private readonly IntegrationTestsFixture _fixture;

        public LessonsControllerTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.LoginApi("user2@AcademyIO.com", "Teste@123");
            _fixture.Client.SetToken(_fixture.Token);

            await _fixture.RegisterStudent2Async();
        }

        [Fact]
        public async Task AddLesson_Success()
        {
            // Arrange
            var courseId = await _fixture.GetIdCourse();
            var lesson = new LessonViewModel
            {
                Name = "Lesson 1",
                CourseId = courseId,
                Subject = "Random",
                TotalHours = 80
            };

            await _fixture.LoginApi();
            _fixture.Client.SetToken(_fixture.Token);

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"/api/lessons", lesson);

            await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async Task StartClass_Forbidden_Failed()
        {
            // Arrange
            var lessonId = await _fixture.GetIdLessonRegistered();

            await _fixture.LoginApi();
            _fixture.Client.SetToken(_fixture.Token);

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"/api/lessons/{lessonId}/start-class", lessonId);

            await response.Content.ReadAsStringAsync();

            // Assert
            string message = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task StartClass_NotRegistered_Failed()
        {
            // Arrange
            await _fixture.LoginApi("user1@AcademyIO.com", "Teste@123");
            _fixture.Client.SetToken(_fixture.Token);
            var lessonId = Guid.NewGuid();

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"/api/lessons/{lessonId}/start-class", lessonId);

            // Assert
            string message = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("\"Você ainda não está matriculado a essa aula.\"", message);
        }

        [Fact]
        public async Task StartClass_Sucess()
        {
            // Arrange
            var lessonId = await _fixture.GetIdLessonNotStarted();

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"/api/lessons/{lessonId}/start-class", lessonId);

            await response.Content.ReadAsStringAsync();

            // Assert
            string message = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task FinishClass_Sucess()
        {
            // Arrange
            var lessonId = await _fixture.GetIdLessonRegistered();
            var paymentViewModel = _fixture.GetPaymentData();

            // Act
            var responseStart = await _fixture.Client.PostAsJsonAsync($"/api/lessons/{lessonId}/start-class", lessonId);
            var response = await _fixture.Client.PostAsJsonAsync($"/api/lessons/{lessonId}/finish-class", lessonId);

            await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task FinishClass_NotStarted_Fail()
        {
            // Arrange
            await _fixture.LoginApi("user1@AcademyIO.com", "Teste@123");
            _fixture.Client.SetToken(_fixture.Token);
   
            await _fixture.RegisterStudent1Lesson1Async();
            var lessonId = await _fixture.GetIdLessonNotStarted();

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"/api/lessons/{lessonId}/finish-class", lessonId);
            string message = await response.Content.ReadAsStringAsync();
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("\"Você ainda não teve progresso nesta aula.\"", message);
        }

        [Fact]
        public async Task FinishClass_NotRegistered_Fail()
        {
            // Arrange
            await _fixture.LoginApi("user1@AcademyIO.com", "Teste@123");
            _fixture.Client.SetToken(_fixture.Token);

            var lessonId = await _fixture.GetIdLessonNotRegistered();

            // Act
            var response = await _fixture.Client.PostAsJsonAsync($"/api/lessons/{lessonId}/finish-class", lessonId);

            await response.Content.ReadAsStringAsync();

            // Assert
            string message = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("\"Você ainda não está matriculado a essa aula.\"", message);
        }

        public Task DisposeAsync()
        {
           return Task.CompletedTask;
        }
    }
}