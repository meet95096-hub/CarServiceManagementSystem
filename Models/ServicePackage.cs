using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.net.Models;

public class ServicePackage
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string Category { get; set; } = "Maintenance";

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Base Labor Price ($)")]
    public decimal BaseLaborPrice { get; set; }

    [Display(Name = "Estimated Hours")]
    public decimal EstimatedHours { get; set; } = 1.5m;

    public bool IsActive { get; set; } = true;

    public string IconClass { get; set; } = "bi-tools";
}
