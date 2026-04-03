using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class SalesOrder
{
    [Key]
    public int Id { get; set; }

    public string? OrderNumber { get; set; }

    public int InquiryId { get; set; }

    [ForeignKey("InquiryId")]
    public SalesInquiry? Inquiry { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    public string? CustomerPONumber { get; set; }

    public string? BillingAddress { get; set; }

    public string? ShippingAddress { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingCharges { get; set; } = 0.00m;

    [Column(TypeName = "decimal(18,2)")]
    public decimal AdvanceReceived { get; set; }

    public string? PaymentTerms { get; set; }

    [Required]
    public string Status { get; set; } = "Confirmed"; // Confirmed, Processing, Shipped, Cancelled

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
