using AcademyIO.Core.DomainObjects.DTOs;

namespace AcademyIO.ManagementPayments.Business;

public interface IPaymentService
{
    Task<bool> MakePaymentCourse(PaymentCourse paymentCourse);
}