using AcademyIO.ManagementPayments.Business;

namespace AcademyIO.ManagementPayments.Application.Query
{
    public class PaymentQuery(IPaymentRepository _repository) : IPaymentQuery
    {
        public async Task<bool> PaymentExists(Guid studentId, Guid courseId)
        {
            return await _repository.PaymentExists(studentId, courseId);
        }
    }
}
