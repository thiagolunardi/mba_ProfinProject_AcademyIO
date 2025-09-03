using Microsoft.EntityFrameworkCore;
using AcademyIO.Core.Data;
using AcademyIO.ManagementPayments.Business;

namespace AcademyIO.ManagementPayments.Data.Repository;

public class PaymentRepository(PaymentsContext context) : IPaymentRepository
{
    private readonly DbSet<Payment> _dbSet = context.Set<Payment>();
    public IUnitOfWork UnitOfWork => context;
    public void Add(Payment payment)
    {
        _dbSet.Add(payment);
    }

    public void AddTransaction(BusinessTransaction transaction)
    {
        context.Set<BusinessTransaction>().Add(transaction);
    }

    public async Task<bool> PaymentExists(Guid studentId, Guid courseId)
    {
       return await _dbSet.AnyAsync(x => x.CourseId == courseId && x.StudentId == studentId);
    }

    public void Dispose()
    {
       context.Dispose();
    }
}