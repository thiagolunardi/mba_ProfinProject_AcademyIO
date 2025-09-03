namespace AcademyIO.ManagementPayments.Application.Query
{
    public interface IPaymentQuery
    {
        Task<bool> PaymentExists(Guid studentId, Guid courseId);
    }
}
