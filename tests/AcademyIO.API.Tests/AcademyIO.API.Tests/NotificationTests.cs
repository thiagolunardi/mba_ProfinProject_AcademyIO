using AcademyIO.Core.Notifications;

namespace AcademyIO.API.Tests;
public class NotificationTests
{
    [Fact]
    public void Constructor_ShouldSetMessageCorrectly()
    {
        // Arrange
        var expectedMessage = "This is a notification";

        // Act
        var notification = new Notification(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, notification.Message);
    }

    [Fact]
    public void Constructor_ShouldAllowEmptyMessage()
    {
        // Act
        var notification = new Notification(string.Empty);

        // Assert
        Assert.Equal(string.Empty, notification.Message);
    }

    [Fact]
    public void Constructor_ShouldAllowNullMessage()
    {
        // Act
        var notification = new Notification(null);

        // Assert
        Assert.Null(notification.Message);
    }
}
