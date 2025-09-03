using AcademyIO.ManagementStudents.Application.Commands;
using AcademyIO.ManagementStudents.Application.Handler;
using AcademyIO.ManagementStudents.Domain;
using FluentAssertions;
using Moq;
using Moq.AutoMock;

namespace AcademyIO.ManagementStudents.Aplication.Tests
{
    public class UserCommandHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly UserCommandHandler _handler;

        public UserCommandHandlerTests()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<UserCommandHandler>();
        }

        [Fact]
        public async Task AddUser_WithSuccess()
        {
            var userId = Guid.NewGuid();
            // Arrange
            var userStudent = new User(userId, "testFabiano", "xxxx", "yyy", "xxxx@hotmail.com", DateTime.Today.AddYears(-50), false);
            var command = new AddUserCommand(userStudent.UserName, userStudent.IsAdmin, userStudent.UserName, userStudent.LastName, userStudent.DateOfBirth, userStudent.Email);

            _mocker.GetMock<IUserRepository>().Setup(x => x.Add(userStudent));
            _mocker.GetMock<IUserRepository>().Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<IUserRepository>().Verify(r => r.Add(It.IsAny<User>()), Times.Once);
            _mocker.GetMock<IUserRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }


        [Fact]
        public async Task AddUser_CommandInvalid_Fail()
        {
            var userId = Guid.NewGuid();
            // Arrange
            var userStudent = new User(userId, "testFabiano", "xxxx", "yyy", "xxxx@hotmail.com", DateTime.Today.AddYears(-50), false);
            var command = new AddUserCommand(string.Empty, userStudent.IsAdmin, string.Empty, string.Empty, userStudent.DateOfBirth, string.Empty);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mocker.GetMock<IUserRepository>().Verify(r => r.Add(It.IsAny<User>()), Times.Never);
            _mocker.GetMock<IUserRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Never);
            Assert.Contains(AddUserCommandValidation.UserNameError,
                command.ValidationResult.Errors.Select(e => e.ErrorMessage));
            Assert.Contains(AddUserCommandValidation.NameError, command.ValidationResult.Errors.Select(e => e.ErrorMessage));
            Assert.Contains(AddUserCommandValidation.EmailError,
                command.ValidationResult.Errors.Select(e => e.ErrorMessage));
            Assert.Contains(AddUserCommandValidation.LastNameError,
                command.ValidationResult.Errors.Select(e => e.ErrorMessage));
            Assert.Equal(4, command.ValidationResult.Errors.Count);
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenCommandIsValid()
        {
            // Arrange
            var cmd = new AddUserCommand(
                userName: "usuario123",
                isAdmin: false,
                name: "Fabiano",
                lastName: "Maciel",
                dateOfBirth: new DateTime(1990, 1, 1),
                email: "fabiano@example.com"
            );

            // Act
            var result = cmd.IsValid();

            // Assert
            result.Should().BeTrue();
            cmd.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenUserNameIsEmpty()
        {
            var cmd = new AddUserCommand(
                userName: "",
                isAdmin: false,
                name: "Fabiano",
                lastName: "Maciel",
                dateOfBirth: new DateTime(1990, 1, 1),
                email: "fabiano@example.com"
            );

            var result = cmd.IsValid();

            result.Should().BeFalse();
            cmd.ValidationResult.Errors.Should().ContainSingle(e => e.PropertyName == "UserName" && e.ErrorMessage == AddUserCommandValidation.UserNameError);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenRequiredFieldsAreEmpty()
        {
            var cmd = new AddUserCommand(
                userName: "",
                isAdmin: false,
                name: "",
                lastName: "",
                dateOfBirth: new DateTime(1990, 1, 1),
                email: ""
            );

            var result = cmd.IsValid();

            result.Should().BeFalse();
            var errors = cmd.ValidationResult.Errors;
            errors.Should().Contain(e => e.PropertyName == "UserName");
            errors.Should().Contain(e => e.PropertyName == "Name");
            errors.Should().Contain(e => e.PropertyName == "LastName");
            errors.Should().Contain(e => e.PropertyName == "Email");
        }
    }
}