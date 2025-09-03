using AcademyIO.ManagementStudents.Aplication.Commands;
using AcademyIO.ManagementStudents.Application.Handler;
using AcademyIO.ManagementStudents.Domain;
using Moq;
using Moq.AutoMock;

namespace AcademyIO.ManagementStudents.Aplication.Tests
{
    public class RegistrationCommandHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly RegistrationCommandHandler _handler;

        public RegistrationCommandHandlerTests()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<RegistrationCommandHandler>();
        }

        [Fact]
        public async Task AddRegistration_WithSuccess()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            // Arrange
            var student = new User(userId, "testFabiano", "xxxx", "yyy", "xxxx@hotmail.com", DateTime.Today.AddYears(-50), false);
            var registration = new Registration(userId, courseId, DateTime.Now);
            var command = new AddRegistrationCommand(userId, courseId);

            _mocker.GetMock<IUserRepository>().Setup(x => x.GetById(command.StudentId)).ReturnsAsync(student);
            _mocker.GetMock<IRegistrationRepository>().Setup(x => x.AddRegistration(userId, courseId)).Returns(registration);
            _mocker.GetMock<IRegistrationRepository>().Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<IRegistrationRepository>().Verify(r => r.AddRegistration(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
            _mocker.GetMock<IRegistrationRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }


        [Fact]
        public async Task AddRegistration_StudentDoesntExist_Fail()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            // Arrange

            var command = new AddRegistrationCommand(userId, courseId);

            _mocker.GetMock<IUserRepository>().Setup(x => x.GetById(command.StudentId)).ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mocker.GetMock<IRegistrationRepository>().Verify(r => r.AddRegistration(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
            _mocker.GetMock<IRegistrationRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Never);
        }

        [Fact]
        public async Task AddRegistration_CommandInvalid_Fail()
        {
            // Arrange
            var command = new AddRegistrationCommand(Guid.Empty, Guid.Empty);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mocker.GetMock<IRegistrationRepository>().Verify(r => r.AddRegistration(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
            _mocker.GetMock<IRegistrationRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Never);
            Assert.Contains(AddRegistrationCommandValidation.StudentIdError,
                command.ValidationResult.Errors.Select(e => e.ErrorMessage));
            Assert.Contains(AddRegistrationCommandValidation.CourseIdError, command.ValidationResult.Errors.Select(e => e.ErrorMessage));
            Assert.Equal(2, command.ValidationResult.Errors.Count);
        }


    }
}