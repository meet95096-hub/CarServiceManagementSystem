using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.net.Models;

public class RepairPartItem
{
    public int Id { get; set; }

    public int RepairJobCardId { get; set; }
    public RepairJobCard? RepairJobCard { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Part Name / Description")]
    public string PartName { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Part Number / SKU")]
    public string? PartNumber { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 100000)]
    [Display(Name = "Unit Price ($)")]
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal LineTotal => Math.Round(Quantity * UnitPrice, 2);
}
