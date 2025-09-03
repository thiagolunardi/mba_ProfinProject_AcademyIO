using AcademyIO.ManagementPayments.Business;
using System.Transactions;

namespace AcademyIO.ManagementPayments.AntiCorruption;

public interface IPayPalGateway
{
    string GetPayPalServiceKey(string apiKey, string encriptionKey);
    string GetCardHashKey(string serviceKey, string cartaoCredito);
    BusinessTransaction CommitTransaction(string cardHashKey, string orderId, double amount);
}