using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using ConcertHub.Web.Models;

var builder = WebApplication.CreateBuilder(args);

string? keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
if (!string.IsNullOrWhiteSpace(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential(),
        new AzureKeyVaultConfigurationOptions());
}

builder.Services.AddControllersWithViews();
builder.Services.Configure<PaymentSettings>(builder.Configuration.GetSection("PaymentSettings"));

string? applicationInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrWhiteSpace(applicationInsightsConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = applicationInsightsConnectionString;
    });

    builder.Logging.AddFilter("Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider", LogLevel.Information);
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
