namespace ConcertHub.Web.Models;

public sealed class PaymentResultViewModel
{
    public string OrderId { get; set; } = string.Empty;

    public string ConcertName { get; set; } = string.Empty;

    public string TicketType { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Total { get; set; }

    public DateTime PaidAtUtc { get; set; }
}
