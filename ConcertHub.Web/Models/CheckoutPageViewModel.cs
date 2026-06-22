namespace ConcertHub.Web.Models;

public sealed class CheckoutPageViewModel
{
    public PaymentRequest Payment { get; set; } = new();

    public bool IsPaymentSecretConfigured { get; set; }

    public bool IsProductionKeyVaultConfigured { get; set; }
}
