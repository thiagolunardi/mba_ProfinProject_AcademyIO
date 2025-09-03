using AcademyIO.ManagementStudents.Domain;

namespace AcademyIO.ManagementCourses.Domain;
public class CertificationTests
{
    [Fact]
    public void Should_Create_Certification_With_Properties()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        // Act
        var certification = new Certification
        {
            CourseId = courseId,
            StudentId = studentId
        };

        // Assert
        Assert.Equal(courseId, certification.CourseId);
        Assert.Equal(studentId, certification.StudentId);
    }
}
