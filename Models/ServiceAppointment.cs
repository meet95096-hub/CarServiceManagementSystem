using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.net.Models;

public class ServiceAppointment
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Appointment Code")]
    public string AppointmentCode { get; set; } = $"APT-{DateTime.UtcNow:yyMMddHHmm}";

    [Required(ErrorMessage = "Vehicle selection is required")]
    [Display(Name = "Vehicle")]
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [Display(Name = "Service Package")]
    public int? ServicePackageId { get; set; }
    public ServicePackage? ServicePackage { get; set; }

    [Required(ErrorMessage = "Service title or reason is required")]
    [StringLength(150)]
    [Display(Name = "Service Requirement")]
    public string ServiceTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Scheduled date is required")]
    [DataType(DataType.Date)]
    [Display(Name = "Scheduled Date")]
    public DateTime ScheduledDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(50)]
    [Display(Name = "Time Slot")]
    public string TimeSlot { get; set; } = "09:00 AM - 11:00 AM";

    [Display(Name = "Assigned Mechanic")]
    public int? MechanicId { get; set; }
    public Mechanic? Mechanic { get; set; }

    [StringLength(50)]
    [Display(Name = "Bay")]
    public string Bay { get; set; } = "Bay 1 - Quick Service";

    [StringLength(500)]
    [Display(Name = "Customer Notes / Symptoms")]
    public string? CustomerNotes { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Estimated Cost ($)")]
    public decimal EstimatedCost { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? LinkedRepairJobId { get; set; }
    public RepairJobCard? LinkedRepairJob { get; set; }
}
