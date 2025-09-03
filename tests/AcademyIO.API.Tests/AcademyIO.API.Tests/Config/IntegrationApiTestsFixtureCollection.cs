using Bogus;
using Bogus.DataSets;
using AcademyIO.Core.Interfaces.Repositories;
using AcademyIO.ManagementCourses.Data;
using AcademyIO.ManagementStudents.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.Api.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static AcademyIO.API.ViewModel.UserViewModel;

namespace AcademyIO.Api.Tests.Config;

[CollectionDefinition(nameof(IntegrationApiTestsFixtureCollection))]
public class IntegrationApiTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture> { }

public class IntegrationTestsFixture : IDisposable
{
    public SqliteConnection Connection { get; set; }

    public readonly AcademyIOAppFactory Factory;
    public HttpClient Client;
    public string ConnectionString { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string PasswordConfirmed { get; set; }
    public string Token { get; set; }
    public PaymentViewModel _paymentViewModel { get; set; }
    public Guid CourseId { get; set; }
    public Guid RegistrationId { get; set; }
    public Guid StudentId { get; set; }
    public Guid LessonId { get; set; }
    //public Guid CertificadoId { get; set; }

    public IntegrationTestsFixture()
    {
        var options = new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost:5224")
        };
        Factory = new AcademyIOAppFactory();
        Client = Factory.CreateClient(options);
        _paymentViewModel = new PaymentViewModel();
        var configuration = Factory.Services.GetRequiredService<IConfiguration>();
        ConnectionString = configuration.GetConnectionString("SQLite") ??
                           throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    }

    public void UserInfo()
    {
        var faker = new Faker("pt_BR");
        UserName = Email = faker.Internet.Email().ToLower();
        Password = faker.Internet.Password(8, false, "", "@1Ab_");
        PasswordConfirmed = Password;
    }

    public PaymentViewModel GetPaymentData()
    {
        var faker = new Faker("pt_BR");
        _paymentViewModel.CardName = faker.Name.FullName();
        _paymentViewModel.CardNumber = faker.Finance.CreditCardNumber(CardType.Mastercard);
        _paymentViewModel.CardExpirationDate = faker.Date.Future(1, DateTime.Now).ToString("MM/yy");
        _paymentViewModel.CardCVV = faker.Finance.CreditCardCvv();

        return _paymentViewModel;
    }

    public void SaveUserToken(string token)
    {
        var response = JsonSerializer.Deserialize<LoginResponseTestViewModel>(token,
             new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new LoginResponseTestViewModel();
        Token = response.Data.AccessToken;
        StudentId = Guid.Parse(response.Data.UserToken.Id);
    }

    public async Task LoginApi(string? email = null, string? password = null)
    {
        var userData = new LoginUserViewModel()
        {
            Email = email ?? "admin@AcademyIO.com",
            Password = password ?? "Teste@123"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", userData);
        response.EnsureSuccessStatusCode();

        SaveUserToken(await response.Content.ReadAsStringAsync());
    }

    public async Task<Guid> GetIdLessonRegistered()
    {
        var response = await Client.GetAsync("/api/Lessons");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();

        var json = JsonSerializer.Deserialize<JsonElement>(data);
        return json.GetProperty("data")[0].GetProperty("id").GetGuid();
    }

    public async Task<Guid> GetIdLessonNotStarted()
    {
        var response = await Client.GetAsync("/api/Lessons");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();

        var json = JsonSerializer.Deserialize<JsonElement>(data);
        return json.GetProperty("data")[1].GetProperty("id").GetGuid();
    }

    public async Task<Guid> GetIdLessonNotRegistered()
    {
        var response = await Client.GetAsync("/api/Lessons");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();

        var json = JsonSerializer.Deserialize<JsonElement>(data);
        return json.GetProperty("data")[0].GetProperty("id").GetGuid();
    }

    public async Task<Guid> GetIdLessonCourseIdRegistered()
    {
        var response = await Client.GetAsync("/api/Lessons");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();

        var json = JsonSerializer.Deserialize<JsonElement>(data);
        return json.GetProperty("data")[0].GetProperty("courseId").GetGuid();
    }

    public async Task<Guid> GetIdCourse()
    {
        var response = await Client.GetAsync("/api/Courses");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();

        var json = JsonSerializer.Deserialize<JsonElement>(data);
        return json.GetProperty("data")[0].GetProperty("id").GetGuid();
    }

    public async Task RegisterStudent2Async()
    {
        var courseLessonId = await GetIdLessonCourseIdRegistered();
        var lessonId = await GetIdLessonRegistered();
        var paymentViewModel = GetPaymentData();

        var responsePayment = await Client.PostAsJsonAsync($"/api/courses/" + courseLessonId + "/make-payment", paymentViewModel);

        bool existsProgress = false;
        using (var scope = Factory.Services.CreateScope())
        {
            var lessonRepository = scope.ServiceProvider.GetRequiredService<ILessonRepository>();
            
            existsProgress = lessonRepository.ExistProgress(lessonId, StudentId);
        }
        if (!existsProgress)
        {
            var responseRegister = await Client.PostAsJsonAsync($"/api/student/register-to-course/{courseLessonId}", courseLessonId);
        }
    }

    public async Task RegisterStudent1Lesson1Async()
    {
        var courseLessonId = await GetIdLessonCourseIdRegistered();
        var lessonId = await GetIdLessonRegistered();
        var paymentViewModel = GetPaymentData();

        var responsePayment = await Client.PostAsJsonAsync($"/api/courses/" + courseLessonId + "/make-payment", paymentViewModel);

        bool existsProgress = false;
        using (var scope = Factory.Services.CreateScope())
        {
            var lessonRepository = scope.ServiceProvider.GetRequiredService<ILessonRepository>();

            existsProgress = lessonRepository.ExistProgress(lessonId, StudentId);
        }
        if (!existsProgress)
        {
            var responseRegister = await Client.PostAsJsonAsync($"/api/student/register-to-course/{courseLessonId}", courseLessonId);
        }
    }

    public void Dispose()
    {
        Factory.Dispose();
        Client.Dispose();
    }
}

public static class TestsExtensions
{
    public static void SetToken(this HttpClient client, string token)
    {
        client.SetJsonMediaType();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static void SetJsonMediaType(this HttpClient client)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}