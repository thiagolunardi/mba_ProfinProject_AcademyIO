using AcademyIO.Core.Messages;

namespace AcademyIO.API.Tests;
public class EventTests
{
    private class TestEvent : Event { }

    [Fact]
    public void Constructor_ShouldSetTimestampToNow()
    {
        // Arrange
        var before = DateTime.Now;

        // Act
        var ev = new TestEvent();

        var after = DateTime.Now;

        // Assert
        Assert.True(ev.Timestamp >= before && ev.Timestamp <= after,
            $"Timestamp ({ev.Timestamp}) should be between {before} and {after}");
    }
}
