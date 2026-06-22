using System.ComponentModel.DataAnnotations;

namespace ConcertHub.Web.Models;

public sealed class PaymentRequest
{
    [Required]
    public string ConcertName { get; set; } = "Neon Harbor Live";

    [Required]
    public string TicketType { get; set; } = "Standard";

    [Range(1, 8)]
    public int Quantity { get; set; } = 2;

    [Required(ErrorMessage = "Вкажіть ім'я покупця.")]
    [StringLength(80)]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть email покупця.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; } = 89m;

    public decimal Total => UnitPrice * Quantity;
}
