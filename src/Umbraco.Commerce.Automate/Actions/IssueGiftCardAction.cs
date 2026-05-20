using System.Globalization;
using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Issues a new gift card.
/// </summary>
[Action("umbracoCommerce.issueGiftCard", "Issue Gift Card",
    Description = "Issues a new gift card (e.g., for a loyalty reward).",
    Group = "Commerce",
    Icon = "icon-gift",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class IssueGiftCardAction : ActionBase<IssueGiftCardSettings, IssueGiftCardOutput>
{
    private readonly IGiftCardService _giftCardService;
    private readonly IUnitOfWorkProvider _uowProvider;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public IssueGiftCardAction(
        ActionInfrastructure infrastructure,
        IGiftCardService giftCardService,
        IUnitOfWorkProvider uowProvider,
        ICommerceStoreAuthorizer storeAuthorizer)
        : base(infrastructure)
    {
        _giftCardService = giftCardService;
        _uowProvider = uowProvider;
        _storeAuthorizer = storeAuthorizer;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<IssueGiftCardSettings>();

        if (!Guid.TryParse(settings.StoreId, out var storeId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Store ID is required."),
                StepRunErrorCategory.Validation);
        }

        var storeAuth = await _storeAuthorizer.AuthorizeStoreAsync(storeId, cancellationToken);
        if (!storeAuth.Authorized)
        {
            return ActionResult.Failed(
                new UnauthorizedAccessException(storeAuth.FailureReason),
                StepRunErrorCategory.Authentication);
        }

        if (!Guid.TryParse(settings.CurrencyId, out var currencyId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Currency ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (settings.Amount <= 0)
        {
            return ActionResult.Failed(
                new ArgumentException("Amount must be greater than zero."),
                StepRunErrorCategory.Validation);
        }

        DateTime? expiryDate = null;
        if (!string.IsNullOrWhiteSpace(settings.ExpiryDate))
        {
            if (!DateTime.TryParse(settings.ExpiryDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed))
            {
                return ActionResult.Failed(
                    new ArgumentException($"'{settings.ExpiryDate}' is not a valid ISO-8601 date."),
                    StepRunErrorCategory.Validation);
            }

            expiryDate = parsed;
        }

        var code = string.IsNullOrWhiteSpace(settings.Code)
            ? await _giftCardService.GenerateGiftCardCodeAsync(storeId)
            : settings.Code;

        GiftCard? giftCard = null;

        await _uowProvider.ExecuteAsync(async uow =>
        {
            giftCard = await GiftCard.CreateAsync(uow, storeId, code, currencyId, settings.Amount);

            if (expiryDate.HasValue)
            {
                await giftCard.SetExpiryDateAsync(expiryDate.Value);
            }

            await _giftCardService.SaveGiftCardAsync(giftCard, cancellationToken);
            uow.Complete();
        }, cancellationToken);

        return Success(new IssueGiftCardOutput
        {
            GiftCardId = giftCard!.Id,
            Code = giftCard.Code,
            Amount = settings.Amount,
            CurrencyId = currencyId,
            StoreId = storeId,
            ExpiryDate = expiryDate,
        });
    }
}
