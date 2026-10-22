using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.net.Models;

public class Invoice
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Invoice #")]
    public string InvoiceNumber { get; set; } = $"INV-{DateTime.UtcNow:yyMMddHHmm}";

    [Required]
    public int RepairJobCardId { get; set; }
    public RepairJobCard? RepairJobCard { get; set; }

    [Required]
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [Display(Name = "Invoice Date")]
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);

    // Amounts
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Labor Total ($)")]
    public decimal LaborTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Parts Total ($)")]
    public decimal PartsTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Subtotal ($)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Discount (%)")]
    public decimal DiscountPercentage { get; set; } = 0m;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Discount Amount ($)")]
    public decimal DiscountAmount { get; set; } = 0m;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tax Rate (%)")]
    public decimal TaxRate { get; set; } = 10.0m; // standard 10%

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tax Amount ($)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Grand Total ($)")]
    public decimal GrandTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Amount Paid ($)")]
    public decimal AmountPaid { get; set; } = 0m;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Balance Due ($)")]
    public decimal BalanceDue { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;

    [StringLength(50)]
    [Display(Name = "Payment Method")]
    public string? PaymentMethod { get; set; }

    [StringLength(100)]
    [Display(Name = "Transaction Reference / Check #")]
    public string? TransactionReference { get; set; }

    public DateTime? PaidAt { get; set; }

    [StringLength(500)]
    [Display(Name = "Notes / Warranty Terms")]
    public string? Notes { get; set; } = "All mechanical labor and genuine OEM parts are covered by 90-day / 5,000 km warranty. Thank you for your business!";
}
