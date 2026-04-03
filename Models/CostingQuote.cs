using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class CostingQuote
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int InquiryId { get; set; }

    [ForeignKey("InquiryId")]
    public SalesInquiry? Inquiry { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal RawMaterialCost { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ProcessingCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Overheads { get; set; }

    [Required]
    public float ProfitMarginPercentage { get; set; }

    public decimal TaxPercentage { get; set; } = 18.00m; // Default 18%

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal FinalPrice { get; set; }

    public decimal FinalPriceWithTax { get; set; }

    public DateTime? ValidityDate { get; set; }

    [Required]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string Status { get; set; } = "Draft"; // Draft, Sent, Accepted, Rejected
}
