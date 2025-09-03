using AcademyIO.API.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using System.Security.Principal;

namespace AcademyIO.API.Tests;
public class AppUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly AppUserService _service;

    public AppUserServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _service = new AppUserService(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void IsAuthenticated_ReturnsTrue_WhenUserIsAuthenticated()
    {
        // Arrange
        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.IsAuthenticated).Returns(true);
        var userMock = new Mock<ClaimsPrincipal>();
        userMock.Setup(u => u.Identity).Returns(identityMock.Object);
        var context = new DefaultHttpContext { User = userMock.Object };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        // Act
        var result = _service.IsAuthenticated();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAuthenticated_ReturnsFalse_WhenUserIsNotAuthenticated()
    {
        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.IsAuthenticated).Returns(false);
        var userMock = new Mock<ClaimsPrincipal>();
        userMock.Setup(u => u.Identity).Returns(identityMock.Object);
        var context = new DefaultHttpContext { User = userMock.Object };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var result = _service.IsAuthenticated();

        Assert.False(result);
    }

    [Fact]
    public void GetId_ReturnsUserId_WhenAuthenticated()
    {
        var userId = Guid.NewGuid().ToString();
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "Test");
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var result = _service.GetId();

        Assert.Equal(Guid.Parse(userId), result);
    }

    [Fact]
    public void GetId_ReturnsNull_WhenNotAuthenticated()
    {
        var identity = new ClaimsIdentity(); // Not authenticated
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var result = _service.GetId();

        Assert.Null(result);
    }

    [Fact]
    public void IsAdmin_ReturnsTrue_WhenUserIsAdmin()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Admin") }, "Test");
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var result = _service.IsAdmin();

        Assert.True(result);
    }

    [Fact]
    public void IsAdmin_ReturnsFalse_WhenUserIsNotAdmin()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "User") }, "Test");
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var result = _service.IsAdmin();

        Assert.False(result);
    }

    [Fact]
    public void IsLoggedUser_ReturnsTrue_WhenUserIdMatches()
    {
        var userId = Guid.NewGuid();
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var result = _service.IsLoggedUser(userId.ToString());

        Assert.True(result);
    }

    [Fact]
    public void IsLoggedUser_ReturnsFalse_WhenUserIdDoesNotMatch()
    {
        var realUserId = Guid.NewGuid();
        var fakeUserId = Guid.NewGuid();
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, realUserId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var result = _service.IsLoggedUser(fakeUserId.ToString());

        Assert.False(result);
    }

    [Fact]
    public void IsLoggedUser_ReturnsFalse_WhenUserIdIsInvalid()
    {
        var result = _service.IsLoggedUser("not-a-guid");

        Assert.False(result);
    }
}
