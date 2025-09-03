using AcademyIO.ManagementPayments.Business;

namespace AcademyIO.ManagementPayments.AntiCorruption;

public class PayPalGateway : IPayPalGateway
{
    public string GetPayPalServiceKey(string apiKey, string encriptionKey)
    {
        return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

    public string GetCardHashKey(string serviceKey, string cartaoCredito)
    {
        return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

    public BusinessTransaction CommitTransaction(string cardHashKey, string orderId, double amount)
    {
        var sucesso = true;
        return new BusinessTransaction
        {
            RegistrationId = Guid.Parse(orderId),
            Total = amount,
            StatusTransaction = sucesso ? StatusTransaction.Accept : StatusTransaction.Declined
        };
    }
}