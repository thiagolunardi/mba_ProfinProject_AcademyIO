using AcademyIO.API.Controllers;
using AcademyIO.API.Extensions;
using AcademyIO.Core.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System.Security.Claims;
using static AcademyIO.API.ViewModel.UserViewModel;

namespace AcademyIO.API.Tests;
public class AuthControllerTests
{
    private readonly Mock<SignInManager<IdentityUser<Guid>>> _signInManagerMock;
    private readonly Mock<UserManager<IdentityUser<Guid>>> _userManagerMock;
    private readonly JwtSettings _jwtSettings;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<INotifier> _notifierMock;

    public AuthControllerTests()
    {
        _signInManagerMock = MockSignInManager();
        _userManagerMock = MockUserManager();
        _jwtSettings = new JwtSettings
        {
            SecretKey = "very_long_secret_key_at_least_32_chars!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationHours = 1
        };
        _mediatorMock = new Mock<IMediator>();
        _notifierMock = new Mock<INotifier>();
    }

    [Fact]
    public async Task Register_ValidModel_ShouldReturnOkWithModel()
    {
        // Arrange
        var model = new RegisterUserViewModel
        {
            Email = "user@test.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            IsAdmin = false
        };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser<Guid>>(), model.Password))
                        .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), "STUDENT"))
                        .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email))
                        .ReturnsAsync(new IdentityUser<Guid> { Id = Guid.NewGuid(), Email = model.Email });

        _userManagerMock.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser<Guid>>()))
                        .ReturnsAsync(new List<Claim>());

        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser<Guid>>()))
                        .ReturnsAsync(new List<string>());

        _mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<Unit>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Unit.Value);

        var controller = new AuthController(
            _signInManagerMock.Object,
            _userManagerMock.Object,
            _jwtSettings,
            _mediatorMock.Object,
            _notifierMock.Object);

        // Act
        var result = await controller.Register(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
    }


    // Helper methods to mock SignInManager and UserManager

    private static Mock<SignInManager<IdentityUser<Guid>>> MockSignInManager()
    {
        var userManager = MockUserManager().Object;
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser<Guid>>>();
        return new Mock<SignInManager<IdentityUser<Guid>>>(userManager, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
    }

    private static Mock<UserManager<IdentityUser<Guid>>> MockUserManager()
    {
        var store = new Mock<IUserStore<IdentityUser<Guid>>>();
        return new Mock<UserManager<IdentityUser<Guid>>>(
            store.Object, null, null, null, null, null, null, null, null);
    }
}
