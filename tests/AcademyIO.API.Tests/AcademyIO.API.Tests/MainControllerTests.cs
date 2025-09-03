using AcademyIO.API.Controllers;
using AcademyIO.Core.Interfaces.Services;
using AcademyIO.Core.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System.Security.Claims;

namespace AcademyIO.API.Tests;
public class TestController : MainController
{
    public TestController(INotifier notifier) : base(notifier) { }

    // Método para expor o CustomResponse para testes
    public ActionResult TestCustomResponse(object result = null) => CustomResponse(result);

    public ActionResult TestCustomResponse(ModelStateDictionary modelState) => CustomResponse(modelState);

    public bool TestIsValid() => IsValid();

    public void TestNotifieError(string message) => NotifieError(message);

    public void TestNotifieErrorInvalidModel(ModelStateDictionary modelState) => NotifieErrorInvalidModel(modelState);
}

public class MainControllerTests
{
    private readonly Mock<INotifier> _notifierMock;
    private readonly TestController _controller;

    public MainControllerTests()
    {
        _notifierMock = new Mock<INotifier>();

        // Setup HttpContext com Claims para UserId funcionar
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }, "mock"));

        _controller = new TestController(_notifierMock.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            }
        };
    }

    [Fact]
    public void UserId_Should_Return_Guid_From_Claims()
    {
        // Act
        var userId = _controller.UserId;

        // Assert
        Assert.NotEqual(Guid.Empty, userId);
    }

    [Fact]
    public void IsValid_Should_Return_True_When_No_Notifications()
    {
        _notifierMock.Setup(n => n.HasNotification()).Returns(false);

        var result = _controller.TestIsValid();

        Assert.True(result);
    }

    [Fact]
    public void IsValid_Should_Return_False_When_Has_Notifications()
    {
        _notifierMock.Setup(n => n.HasNotification()).Returns(true);

        var result = _controller.TestIsValid();

        Assert.False(result);
    }

    [Fact]
    public void CustomResponse_Should_Return_Ok_With_Success_True_When_Valid()
    {
        _notifierMock.Setup(n => n.HasNotification()).Returns(false);

        var obj = new { Name = "Test" };

        var result = _controller.TestCustomResponse(obj) as OkObjectResult;

        Assert.NotNull(result);
        Assert.True((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value));
        Assert.Equal(obj, result.Value.GetType().GetProperty("data").GetValue(result.Value));
    }

    [Fact]
    public void NotifieError_Should_Call_Handle_Of_Notifier()
    {
        var msg = "Test Error";
        _controller.TestNotifieError(msg);

        _notifierMock.Verify(n => n.Handle(It.Is<Notification>(no => no.Message == msg)), Times.Once);
    }

    [Fact]
    public void NotifieErrorInvalidModel_Should_Call_NotifieError_For_Each_ModelError()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("key1", "error1");
        modelState.AddModelError("key2", "error2");

        _controller.TestNotifieErrorInvalidModel(modelState);

        _notifierMock.Verify(n => n.Handle(It.IsAny<Notification>()), Times.Exactly(2));
    }
}
