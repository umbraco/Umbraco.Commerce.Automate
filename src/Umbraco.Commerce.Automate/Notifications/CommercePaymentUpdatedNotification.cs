using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Common.Models;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderTransactionUpdatedNotification.
/// </summary>
public sealed class CommercePaymentUpdatedNotification : INotification
{
    public OrderReadOnly Order { get; }
    public ChangingValue<PaymentStatus?> PaymentStatus { get; }
    public ChangingValue<string> TransactionId { get; }
    public ChangingValue<decimal> AmountAuthorized { get; }
    public ChangingValue<decimal> TransactionFee { get; }

    public CommercePaymentUpdatedNotification(
        OrderReadOnly order,
        ChangingValue<PaymentStatus?> paymentStatus,
        ChangingValue<string> transactionId,
        ChangingValue<decimal> amountAuthorized,
        ChangingValue<decimal> transactionFee)
    {
        Order = order;
        PaymentStatus = paymentStatus;
        TransactionId = transactionId;
        AmountAuthorized = amountAuthorized;
        TransactionFee = transactionFee;
    }
}
