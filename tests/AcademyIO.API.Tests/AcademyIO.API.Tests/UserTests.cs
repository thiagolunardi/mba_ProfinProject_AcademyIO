using AcademyIO.Core.Models;

namespace AcademyIO.API.Tests;
public class UserTests
{
    [Fact]
    public void Create_ShouldReturnUserWithCorrectProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "john.doe@example.com";
        var firstName = "John";
        var lastName = "Doe";
        var birthdate = new DateTime(1990, 1, 1);

        // Act
        var user = User.Create(id, email, firstName, lastName, birthdate);

        // Assert
        Assert.Equal(id, user.Id); // Assumindo que 'Entity' define Id
        Assert.Equal(firstName, user.FistName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(email, user.Email);
        Assert.Equal(birthdate, user.Birthdate);
    }
}
