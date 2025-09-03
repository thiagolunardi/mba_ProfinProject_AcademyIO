using Microsoft.Extensions.Options;
using AcademyIO.ManagementPayments.Business;

namespace AcademyIO.ManagementPayments.AntiCorruption;

public class PaymentCreditCardFacade(IPayPalGateway payPalGateway,
    IOptions<PaymentSettings> options) : IPaymentCreditCardFacade
{
    private readonly PaymentSettings _settings = options.Value;
    public BusinessTransaction MakePayment(Payment payment)
    {
        var apiKey = _settings.ApiKey;
        var encriptionKey = _settings.EncriptionKey;

        var serviceKey = payPalGateway.GetPayPalServiceKey(apiKey, encriptionKey);
        var cardHashKey = payPalGateway.GetCardHashKey(serviceKey, payment.CardNumber);

        var transaction = payPalGateway.CommitTransaction(cardHashKey, payment.CourseId.ToString(), payment.Value);

        transaction.PaymentId = payment.Id;

        return transaction;
    }
}