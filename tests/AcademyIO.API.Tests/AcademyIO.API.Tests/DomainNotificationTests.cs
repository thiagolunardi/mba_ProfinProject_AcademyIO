using AcademyIO.Core.Messages.Notifications;

namespace AcademyIO.API.Tests;
public class DomainNotificationTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var key = "Error";
        var value = "Something went wrong";
        var before = DateTime.Now;

        // Act
        var notification = new DomainNotification(key, value);
        var after = DateTime.Now;

        // Assert
        Assert.Equal(key, notification.Key);
        Assert.Equal(value, notification.Value);
        Assert.Equal(1, notification.Version);
        Assert.NotEqual(Guid.Empty, notification.DomainNotificationId);
        Assert.InRange(notification.Timestamp, before, after);
    }

    [Fact]
    public void MultipleInstances_ShouldHaveDifferentGuids()
    {
        // Act
        var n1 = new DomainNotification("key1", "value1");
        var n2 = new DomainNotification("key2", "value2");

        // Assert
        Assert.NotEqual(n1.DomainNotificationId, n2.DomainNotificationId);
    }
}
