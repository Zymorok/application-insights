using System.Diagnostics;
using ConcertHub.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConcertHub.Web.Controllers;

public class HomeController : Controller
{
    private static readonly IReadOnlyDictionary<string, decimal> TicketPrices = new Dictionary<string, decimal>
    {
        ["Standard"] = 89m,
        ["Fan Zone"] = 129m,
        ["Backstage"] = 159m
    };

    private readonly PaymentSettings paymentSettings;
    private readonly IConfiguration configuration;
    private readonly ILogger<HomeController> logger;

    public HomeController(
        IOptions<PaymentSettings> paymentSettings,
        IConfiguration configuration,
        ILogger<HomeController> logger)
    {
        this.paymentSettings = paymentSettings.Value;
        this.configuration = configuration;
        this.logger = logger;
    }

    public IActionResult Index()
    {
        return View(CreateCheckoutPage(new PaymentRequest()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Pay(CheckoutPageViewModel model)
    {
        PaymentRequest payment = model.Payment;
        if (!TicketPrices.TryGetValue(payment.TicketType, out decimal unitPrice))
        {
            ModelState.AddModelError("Payment.TicketType", "Оберіть коректний тип квитка.");
        }

        payment.UnitPrice = unitPrice;

        if (!ModelState.IsValid)
        {
            return View("Index", CreateCheckoutPage(payment));
        }

        if (!paymentSettings.HasValidToken())
        {
            logger.LogError(
                "Payment rejected for {ConcertName}. PaymentSettings:ApiSecretToken is missing or still uses the default value.",
                payment.ConcertName);

            ModelState.AddModelError(
                string.Empty,
                "Платіжний секрет не налаштовано. Додайте PaymentSettings:ApiSecretToken через User Secrets або Azure Key Vault.");

            return View("Index", CreateCheckoutPage(payment));
        }

        string orderId = $"CH-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";

        logger.LogInformation(
            "Payment succeeded. OrderId={OrderId}, Concert={ConcertName}, TicketType={TicketType}, Quantity={Quantity}, Total={Total}.",
            orderId,
            payment.ConcertName,
            payment.TicketType,
            payment.Quantity,
            payment.Total);

        return View("PaymentSuccess", new PaymentResultViewModel
        {
            OrderId = orderId,
            ConcertName = payment.ConcertName,
            TicketType = payment.TicketType,
            Quantity = payment.Quantity,
            Total = payment.Total,
            PaidAtUtc = DateTime.UtcNow
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private CheckoutPageViewModel CreateCheckoutPage(PaymentRequest payment)
    {
        return new CheckoutPageViewModel
        {
            Payment = NormalizePayment(payment),
            IsPaymentSecretConfigured = paymentSettings.HasValidToken(),
            IsProductionKeyVaultConfigured = !string.IsNullOrWhiteSpace(configuration["KeyVault:VaultUri"])
        };
    }

    private static PaymentRequest NormalizePayment(PaymentRequest payment)
    {
        payment.UnitPrice = TicketPrices.TryGetValue(payment.TicketType, out decimal unitPrice)
            ? unitPrice
            : TicketPrices["Standard"];

        return payment;
    }
}
