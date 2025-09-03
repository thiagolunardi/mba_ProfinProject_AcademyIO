using AcademyIO.Core.Interfaces.Repositories;
using AcademyIO.ManagementCourses.Application.Commands;
using AcademyIO.ManagementCourses.Application.Handlers;
using AcademyIO.ManagementCourses.Domain;
using Moq;
using Moq.AutoMock;

namespace AcademyIO.ManagementCourses.Aplication.Tests;

public class LessonCommandHandlerTests
{
    private readonly AutoMocker _mocker;
    private readonly LessonCommandHandler _handler;

    public LessonCommandHandlerTests()
    {
        _mocker = new AutoMocker();
        _handler = _mocker.CreateInstance<LessonCommandHandler>();
    }

    [Fact]
    public async Task CreateLesson_Sucess()
    {
        // Arrange
        var command = new AddLessonCommand("Lesson 1", "yyyyy", Guid.NewGuid(), 80);

        var lesson = new Lesson(command.Name, command.Subject, command.TotalHours, command.CourseId);
        _mocker.GetMock<ILessonRepository>().Setup(x => x.Add(lesson));
        _mocker.GetMock<ILessonRepository>().Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mocker.GetMock<ILessonRepository>().Verify(r => r.Add(It.IsAny<Lesson>()), Times.Once);
        _mocker.GetMock<ILessonRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task Create_CommandInvalid_Failed()
    {
        // Arrange
        var command = new AddLessonCommand("", "", Guid.Empty, 0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _mocker.GetMock<ILessonRepository>().Verify(r => r.Add(It.IsAny<Lesson>()), Times.Never);
        _mocker.GetMock<ILessonRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Never);
    }

    [Fact]
    public async Task StartProgress_Success()
    {
        var lessonId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        // Arrange
        var command = new StartLessonCommand(lessonId, studentId);
        _mocker.GetMock<ILessonRepository>().Setup(x => x.StartLesson(command.LessonId, command.StudentId)).Returns(Task.FromResult(true));
        _mocker.GetMock<ILessonRepository>().Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mocker.GetMock<ILessonRepository>().Verify(r => r.StartLesson(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        _mocker.GetMock<ILessonRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task FinishProgress_Success()
    {
        var lessonId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        // Arrange
        var command = new FinishLessonCommand(lessonId, studentId);
        _mocker.GetMock<ILessonRepository>().Setup(x => x.FinishLesson(command.LessonId, command.StudentId)).Returns(Task.FromResult(true));
        _mocker.GetMock<ILessonRepository>().Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mocker.GetMock<ILessonRepository>().Verify(r => r.FinishLesson(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        _mocker.GetMock<ILessonRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task StartLesson_CommandInvalid_Fail()
    {
        // Arrange
        var command = new StartLessonCommand(Guid.Empty, Guid.Empty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _mocker.GetMock<ILessonRepository>().Verify(r => r.StartLesson(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mocker.GetMock<ILessonRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Never);
        Assert.Contains(StartLessonCommandValidation.LessonIdError,
            command.ValidationResult.Errors.Select(e => e.ErrorMessage));
        Assert.Contains(StartLessonCommandValidation.StudentIdError, command.ValidationResult.Errors.Select(e => e.ErrorMessage));
        Assert.Equal(2, command.ValidationResult.Errors.Count);
    }

    [Fact]
    public async Task StartLesson_Finished_Fail()
    {
        // Arrange
        var command = new StartLessonCommand(Guid.Empty, Guid.Empty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _mocker.GetMock<ILessonRepository>().Verify(r => r.StartLesson(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        _mocker.GetMock<ILessonRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Never);
        Assert.Contains(StartLessonCommandValidation.LessonIdError,
            command.ValidationResult.Errors.Select(e => e.ErrorMessage));
        Assert.Contains(StartLessonCommandValidation.StudentIdError, command.ValidationResult.Errors.Select(e => e.ErrorMessage));
        Assert.Equal(2, command.ValidationResult.Errors.Count);
    }
}