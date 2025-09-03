using System;
using FluentAssertions;
using Xunit;
using AcademyIO.Core.Messages.IntegrationCommands;
using AcademyIO.Core.DomainObjects.DTOs;
using Bogus.DataSets;
using Bogus;
using PlataformaEducacao.Api.DTOs;

namespace AcademyIO.API.Tests;
public class MakePaymentCourseCommandTests
{
    [Fact]
    public void IsValid_ShouldReturnTrue_WhenPaymentCourseIsNotNull()
    {
        var faker = new Faker("pt_BR");

        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 100,
            CardCVV = faker.Finance.CreditCardCvv(),
            CardNumber = faker.Finance.CreditCardNumber(CardType.Mastercard),
            CardExpirationDate = faker.Date.Future(1, DateTime.Now).ToString("MM/yy"),
            CardName = faker.Name.FullName()
        };

        var command = new MakePaymentCourseCommand(paymentCourse);

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeTrue();
        command.ValidationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenPaymentCourseIsNull()
    {
        // Arrange
        MakePaymentCourseCommand command = new MakePaymentCourseCommand(null);

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().BeFalse();
        command.ValidationResult.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be(MakePaymentCourseCommandValidation.PaymentCourseError);
    }
}
