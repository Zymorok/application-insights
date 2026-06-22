namespace ConcertHub.Web.Models;

public sealed class PaymentSettings
{
    public const string DefaultTokenValue = "CHANGE_ME";

    public string ApiSecretToken { get; set; } = string.Empty;

    public bool HasValidToken()
    {
        return !string.IsNullOrWhiteSpace(ApiSecretToken)
            && !string.Equals(ApiSecretToken, DefaultTokenValue, StringComparison.OrdinalIgnoreCase);
    }
}
