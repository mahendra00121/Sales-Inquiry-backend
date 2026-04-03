using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolyTrack.API.Models;

public class Packing
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FinalQCId { get; set; }

    [ForeignKey("FinalQCId")]
    public FinalQC? FinalQCRecord { get; set; }

    [Required]
    public int NumberOfBoxes { get; set; }

    public string? PackingType { get; set; } // Wood, Plastic, Cardboard

    [Required]
    public decimal TotalWeight { get; set; }

    [Required]
    public string PackedBy { get; set; } = string.Empty;

    public DateTime PackingDate { get; set; } = DateTime.Now;

    public string? PackingNotes { get; set; }
}
