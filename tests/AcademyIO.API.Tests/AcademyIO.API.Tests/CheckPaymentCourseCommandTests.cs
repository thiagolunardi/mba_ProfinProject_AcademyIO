using AcademyIO.Core.Messages.IntegrationCommands;
using FluentAssertions;

namespace AcademyIO.API.Tests;
public class CheckPaymentCourseCommandTests
{
    [Fact]
    public void IsValid_ShouldReturnTrue_WhenCommandIsValid()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var command = new CheckPaymentCourseCommand(studentId, courseId);

        // Act
        var isValid = command.IsValid();

        // Assert
        isValid.Should().BeTrue();
        command.ValidationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenStudentIdIsEmpty()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var command = new CheckPaymentCourseCommand(Guid.Empty, courseId);

        // Act
        var isValid = command.IsValid();

        // Assert
        isValid.Should().BeFalse();
        command.ValidationResult.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be(CheckPaymentCourseCommandValidation.StudentIdError);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenCourseIdIsEmpty()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var command = new CheckPaymentCourseCommand(studentId, Guid.Empty);

        // Act
        var isValid = command.IsValid();

        // Assert
        isValid.Should().BeFalse();
        command.ValidationResult.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be(CheckPaymentCourseCommandValidation.CourseIdError);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenBothIdsAreEmpty()
    {
        // Arrange
        var command = new CheckPaymentCourseCommand(Guid.Empty, Guid.Empty);

        // Act
        var isValid = command.IsValid();

        // Assert
        isValid.Should().BeFalse();
        command.ValidationResult.Errors.Should().HaveCount(2);
        command.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == CheckPaymentCourseCommandValidation.StudentIdError);
        command.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == CheckPaymentCourseCommandValidation.CourseIdError);
    }
}
