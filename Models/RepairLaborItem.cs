using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.net.Models;

public class RepairLaborItem
{
    public int Id { get; set; }

    public int RepairJobCardId { get; set; }
    public RepairJobCard? RepairJobCard { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0.1, 100)]
    public decimal Hours { get; set; } = 1.0m;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 10000)]
    [Display(Name = "Hourly Rate ($)")]
    public decimal HourlyRate { get; set; } = 85.00m;

    [NotMapped]
    public decimal LineTotal => Math.Round(Hours * HourlyRate, 2);
}
