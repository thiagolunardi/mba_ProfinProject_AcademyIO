using AcademyIO.Core.Data;

namespace AcademyIO.ManagementPayments.Business;

public interface IPaymentRepository : IRepository<Payment>
{
    void Add(Payment payment);
    void AddTransaction(BusinessTransaction transaction);

    Task<bool> PaymentExists(Guid studentId, Guid courseId);
}