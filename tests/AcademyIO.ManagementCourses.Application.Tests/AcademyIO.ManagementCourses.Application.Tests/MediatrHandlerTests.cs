using AcademyIO.Core.Bus;
using AcademyIO.Core.Messages;
using MediatR;
using Moq;

namespace AcademyIO.ManagementCourses.Aplication.Tests
{
    public class MediatrHandlerTests
    {
        [Fact]
        public async Task PublicEvent_ShouldCallMediatorPublish()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var handler = new MediatrHandler(mediatorMock.Object);
            var testEvent = new TestEvent(); // Classe derivada de Event

            // Act
            await handler.PublicEvent(testEvent);

            // Assert
            mediatorMock.Verify(m => m.Publish(testEvent, default), Times.Once);
        }

        private class TestEvent : Event { }
    }
}
