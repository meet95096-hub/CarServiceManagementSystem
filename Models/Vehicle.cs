using System.ComponentModel.DataAnnotations;

namespace web.net.Models;

public class Vehicle
{
    public int Id { get; set; }

    [Required(ErrorMessage = "License plate number is required")]
    [StringLength(20)]
    [Display(Name = "License / Plate #")]
    public string LicensePlate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vehicle make is required")]
    [StringLength(50)]
    public string Make { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vehicle model is required")]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Range(1960, 2030, ErrorMessage = "Please enter a valid year between 1960 and 2030")]
    public int Year { get; set; } = DateTime.Now.Year;

    [StringLength(30)]
    public string Color { get; set; } = "Black";

    [StringLength(30)]
    [Display(Name = "VIN / Chassis #")]
    public string? Vin { get; set; }

    [Range(0, 2000000)]
    [Display(Name = "Mileage (km/mi)")]
    public int CurrentMileage { get; set; }

    [StringLength(20)]
    [Display(Name = "Fuel Type")]
    public string FuelType { get; set; } = "Gasoline";

    [StringLength(20)]
    public string Transmission { get; set; } = "Automatic";

    // Owner / Customer details
    [Required(ErrorMessage = "Customer / Owner name is required")]
    [StringLength(100)]
    [Display(Name = "Owner Name")]
    public string OwnerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Owner phone number is required")]
    [StringLength(25)]
    [Phone]
    [Display(Name = "Owner Phone")]
    public string OwnerPhone { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(100)]
    [Display(Name = "Owner Email")]
    public string? OwnerEmail { get; set; }

    [StringLength(200)]
    [Display(Name = "Owner Address")]
    public string? OwnerAddress { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<ServiceAppointment> Appointments { get; set; } = new List<ServiceAppointment>();
    public ICollection<RepairJobCard> RepairJobs { get; set; } = new List<RepairJobCard>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public string DisplayTitle => $"{Year} {Make} {Model} ({LicensePlate})";
}
