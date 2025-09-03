using AcademyIO.Core.Interfaces.Repositories;
using AcademyIO.ManagementCourses.Application.Commands;
using AcademyIO.ManagementCourses.Application.Handlers;
using AcademyIO.ManagementCourses.Domain;
using Moq;
using Moq.AutoMock;

namespace AcademyIO.ManagementCourses.Aplication.Tests
{
    public class CourseCommandHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly CourseCommandHandler _handler;

        public CourseCommandHandlerTests()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<CourseCommandHandler>();
        }

        [Fact]
        public async Task CreateCourse_WithSuccess()
        {
            // Arrange
            var command = new AddCourseCommand("Test course", "test context", Guid.NewGuid(), 350);

            _mocker.GetMock<ICourseRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<ICourseRepository>().Verify(r => r.Add(It.IsAny<Course>()), Times.Once);
            _mocker.GetMock<ICourseRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }

        [Fact]
        public async Task CreateCourse_Fail()
        {
            // Arrange
            var command = new AddCourseCommand("", "", Guid.Empty, 0);

            _mocker.GetMock<ICourseRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(false));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);

            _mocker.GetMock<ICourseRepository>().Verify(r => r.Add(It.IsAny<Course>()), Times.Never);
            _mocker.GetMock<ICourseRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Never);
        }

        [Fact]
        public async Task CreateProgressByCourse_Success()
        {
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            // Arrange
            var command = new CreateProgressByCourseCommand(courseId, studentId);
            _mocker.GetMock<ILessonRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);

            _mocker.GetMock<ILessonRepository>().Verify(r => r.CreateProgressLessonByCourse(courseId, studentId), Times.Once);
            _mocker.GetMock<ILessonRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }

        [Fact]
        public async Task CreateProgressByCourse_Fail()
        {
            var courseId = Guid.Empty;
            var studentId = Guid.NewGuid();

            // Arrange
            var command = new CreateProgressByCourseCommand(courseId, studentId);
            _mocker.GetMock<ILessonRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(false));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);

            _mocker.GetMock<ILessonRepository>().Verify(r => r.CreateProgressLessonByCourse(courseId, studentId), Times.Never);
            _mocker.GetMock<ILessonRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Never);
        }
    }
}