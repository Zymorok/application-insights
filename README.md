# Application Insights / ConcertHub.Web

ASP.NET Core MVC project for the enterprise security homework: a ConcertHub checkout page that reads the payment provider secret from secure configuration instead of project files.

## What Was Implemented

- Payment simulation for ticket checkout.
- `PaymentSettings:ApiSecretToken` is read through `IConfiguration`.
- Empty/default payment secret blocks checkout and writes an error log.
- Valid secret allows checkout and writes an information log for Application Insights.
- Local development token is stored with .NET User Secrets.
- Production secrets can be loaded from Azure Key Vault through Managed Identity.

## Local Secret

The local secret was configured with:

```bash
dotnet user-secrets set "PaymentSettings:ApiSecretToken" "YOUR_LOCAL_PAYMENT_TOKEN"
```

`appsettings.json` intentionally contains an empty value and no real token.

## Azure Key Vault Secret

Create this secret in Azure Key Vault:

- Name: `PaymentSettings--ApiSecretToken`
- Value: your long production payment token

If Application Insights is also stored in Key Vault, use:

- Name: `ApplicationInsights--ConnectionString`
- Value: your Application Insights connection string

Configure the App Service setting:

- `KeyVault__VaultUri=https://YOUR-VAULT-NAME.vault.azure.net/`

The app uses `DefaultAzureCredential`, so Azure App Service should read Key Vault through Managed Identity.

## Azure Deployment

- Web App: `concerthub-vitalii-20260622`
- URL: `https://concerthub-vitalii-20260622.azurewebsites.net/`
- Key Vault: `vitalii6307299003`
- Secrets configured:
  - `ApplicationInsights--ConnectionString`
  - `PaymentSettings--ApiSecretToken`
- Managed Identity access: Key Vault `get` and `list` secret permissions.
- Application Insights contains `Payment succeeded` information logs for successful checkout attempts.

## Run

```bash
dotnet run --project ConcertHub.Web --urls http://localhost:5090
```

Open `http://localhost:5090`, fill the checkout form, and submit payment.

## Submission Checklist

- GitHub repository link.
- Screenshot of Azure Key Vault `Secrets` showing Application Insights and payment secret.
- Screenshot of successful payment from the deployed `.azurewebsites.net` site.
