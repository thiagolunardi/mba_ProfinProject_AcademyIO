using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AcademyIO.API.Controllers;
using AcademyIO.Core.Interfaces.Services;
using AcademyIO.ManagementCourses.Application.Queries;
using AcademyIO.ManagementPayments.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AcademyIO.ManagementCourses.Application.Commands;
using AcademyIO.ManagementStudents.Aplication.Commands;
using AcademyIO.ManagementCourses.Application.Queries.ViewModels;

namespace AcademyIO.API.Tests;
public class StudentControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICourseQuery> _courseQueryMock;
    private readonly Mock<IPaymentQuery> _paymentQueryMock;
    private readonly Mock<INotifier> _notifierMock;
    private readonly StudentController _controller;
    private readonly Guid _userId;

    public StudentControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _courseQueryMock = new Mock<ICourseQuery>();
        _paymentQueryMock = new Mock<IPaymentQuery>();
        _notifierMock = new Mock<INotifier>();

        _userId = Guid.NewGuid();

        // Simula o usuário autenticado com o UserId no claim
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        }));

        _controller = new StudentController(_mediatorMock.Object,
                                            _courseQueryMock.Object,
                                            _paymentQueryMock.Object,
                                            _notifierMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task RegisterToCourse_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        _courseQueryMock.Setup(c => c.GetById(It.IsAny<Guid>())).ReturnsAsync((CourseViewModel)null);

        // Act
        var result = await _controller.RegisterToCourse(Guid.NewGuid());

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Curso não encontrado.", notFoundResult.Value);
    }

    [Fact]
    public async Task RegisterToCourse_ReturnsUnprocessableEntity_WhenPaymentDoesNotExist()
    {
        // Arrange
        _courseQueryMock.Setup(c => c.GetById(It.IsAny<Guid>())).ReturnsAsync(new CourseViewModel());
        _paymentQueryMock.Setup(p => p.PaymentExists(_userId, It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var result = await _controller.RegisterToCourse(Guid.NewGuid());

        // Assert
        var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.Equal("Você não possui acesso a esse curso.", unprocessableResult.Value);
    }

    [Fact]
    public async Task RegisterToCourse_ReturnsCreated_WhenAllOk()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseQueryMock.Setup(c => c.GetById(courseId)).ReturnsAsync(new CourseViewModel());
        _paymentQueryMock.Setup(p => p.PaymentExists(_userId, courseId)).ReturnsAsync(true);

        _mediatorMock
      .Setup(m => m.Send(It.IsAny<AddRegistrationCommand>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProgressByCourseCommand>(), default)).ReturnsAsync(true);

        // Act
        var result = await _controller.RegisterToCourse(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
 
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    // GetRegistration tem a mesma lógica do RegisterToCourse, então testes são similares:

    [Fact]
    public async Task GetRegistration_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        _courseQueryMock.Setup(c => c.GetById(It.IsAny<Guid>())).ReturnsAsync((CourseViewModel)null);
        var result = await _controller.GetRegistration(Guid.NewGuid());
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Curso não encontrado.", notFoundResult.Value);
    }

    [Fact]
    public async Task GetRegistration_ReturnsUnprocessableEntity_WhenPaymentDoesNotExist()
    {
        _courseQueryMock.Setup(c => c.GetById(It.IsAny<Guid>())).ReturnsAsync(new CourseViewModel());
        _paymentQueryMock.Setup(p => p.PaymentExists(_userId, It.IsAny<Guid>())).ReturnsAsync(false);
        var result = await _controller.GetRegistration(Guid.NewGuid());
        var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.Equal("Você não possui acesso a esse curso.", unprocessableResult.Value);
    }

    [Fact]
    public async Task GetRegistration_ReturnsCreated_WhenAllOk()
    {
        var courseId = Guid.NewGuid();
        _courseQueryMock.Setup(c => c.GetById(courseId)).ReturnsAsync(new CourseViewModel());
        _paymentQueryMock.Setup(p => p.PaymentExists(_userId, courseId)).ReturnsAsync(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<AddRegistrationCommand>(), default)).ReturnsAsync(true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProgressByCourseCommand>(), default)).ReturnsAsync(true);

        var result = await _controller.GetRegistration(courseId);

        var okResult = Assert.IsType<OkObjectResult>(result);
  
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }
}
