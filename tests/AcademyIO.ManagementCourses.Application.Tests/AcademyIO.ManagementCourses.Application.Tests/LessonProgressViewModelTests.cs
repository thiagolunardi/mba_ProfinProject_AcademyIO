using AcademyIO.ManagementCourses.Application.Queries;

namespace AcademyIO.ManagementCourses.Aplication.Tests;
public class LessonProgressViewModelTests
{
    [Fact]
    public void Constructor_ShouldAssignProperties()
    {
        // Arrange
        var lessonName = "Lesson 1";
        var progressLesson = "50%";

        // Act
        var viewModel = new LessonProgressViewModel(lessonName, progressLesson);

        // Assert
        Assert.Equal(lessonName, viewModel.LessonName);
        Assert.Equal(progressLesson, viewModel.ProgressLesson);
    }
}
