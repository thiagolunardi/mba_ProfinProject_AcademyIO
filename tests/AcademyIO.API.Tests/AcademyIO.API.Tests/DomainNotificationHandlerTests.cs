using AcademyIO.Core.Messages.Notifications;

namespace AcademyIO.API.Tests;
public class DomainNotificationHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAddNotification()
    {
        // Arrange
        var handler = new DomainNotificationHandler();
        var notification = new DomainNotification("Error", "Something went wrong");

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        var notifications = handler.GetNotifications();
        Assert.Single(notifications);
        Assert.Equal("Error", notifications[0].Key);
        Assert.Equal("Something went wrong", notifications[0].Value);
    }

    [Fact]
    public void HasNotification_ShouldReturnFalse_WhenNoNotifications()
    {
        // Arrange
        var handler = new DomainNotificationHandler();

        // Act
        var result = handler.HasNotification();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasNotification_ShouldReturnTrue_WhenThereIsNotification()
    {
        // Arrange
        var handler = new DomainNotificationHandler();
        var notification = new DomainNotification("Warning", "Be careful");

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        Assert.True(handler.HasNotification());
    }

    [Fact]
    public async Task Dispose_ShouldClearNotifications()
    {
        // Arrange
        var handler = new DomainNotificationHandler();
        var notification = new DomainNotification("Error", "Clear this");

        await handler.Handle(notification, CancellationToken.None);

        // Act
        handler.Dispose();

        // Assert
        Assert.Empty(handler.GetNotifications());
    }
}
