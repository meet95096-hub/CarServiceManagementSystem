using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.net.Models;

public class RepairJobCard
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Job Card #")]
    public string JobCardNumber { get; set; } = $"JOB-{DateTime.UtcNow:yyMMddHHmm}";

    [Required]
    [Display(Name = "Vehicle")]
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [Display(Name = "Originating Appointment")]
    public int? ServiceAppointmentId { get; set; }
    public ServiceAppointment? ServiceAppointment { get; set; }

    [Display(Name = "Lead Technician")]
    public int? LeadMechanicId { get; set; }
    public Mechanic? LeadMechanic { get; set; }

    [StringLength(50)]
    [Display(Name = "Service Bay")]
    public string Bay { get; set; } = "Bay 1";

    [Display(Name = "Odometer at Check-In (km/mi)")]
    public int OdometerIn { get; set; }

    [StringLength(1000)]
    [Display(Name = "Initial Diagnostic Findings")]
    public string? DiagnosticNotes { get; set; }

    [StringLength(1000)]
    [Display(Name = "Work Performed / Recommendations")]
    public string? WorkPerformedSummary { get; set; }

    public RepairStatus Status { get; set; } = RepairStatus.PendingCheckIn;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Estimated Completion Time")]
    public DateTime? EstimatedCompletionAt { get; set; }

    [Display(Name = "Actual Delivery Time")]
    public DateTime? CompletedAt { get; set; }

    // Multi-Point Inspection Checklist
    public bool CheckEngineOil { get; set; } = true;
    public bool CheckBrakeSystem { get; set; } = true;
    public bool CheckBatteryHealth { get; set; } = true;
    public bool CheckTirePressureTread { get; set; } = true;
    public bool CheckCoolantRadiator { get; set; } = true;
    public bool CheckSuspensionSteering { get; set; } = true;
    public bool CheckLightsElectronics { get; set; } = true;

    // Inspection Status Summary: Pass, Attention Needed, Critical
    [StringLength(30)]
    public string OverallInspectionGrade { get; set; } = "Pass - Good Condition";

    // Navigation
    public ICollection<RepairLaborItem> LaborItems { get; set; } = new List<RepairLaborItem>();
    public ICollection<RepairPartItem> PartItems { get; set; } = new List<RepairPartItem>();
    public Invoice? Invoice { get; set; }

    [NotMapped]
    public decimal TotalLaborCost => LaborItems?.Sum(l => l.LineTotal) ?? 0m;

    [NotMapped]
    public decimal TotalPartsCost => PartItems?.Sum(p => p.LineTotal) ?? 0m;

    [NotMapped]
    public decimal EstimatedTotalCost => TotalLaborCost + TotalPartsCost;

    [NotMapped]
    public int ProgressPercentage => Status switch
    {
        RepairStatus.PendingCheckIn => 10,
        RepairStatus.InspectionDiagnostic => 30,
        RepairStatus.InProgress => 55,
        RepairStatus.WaitingParts => 65,
        RepairStatus.QualityCheck => 85,
        RepairStatus.ReadyForDelivery => 95,
        RepairStatus.Delivered => 100,
        _ => 0
    };

    [NotMapped]
    public string StatusBadgeClass => Status switch
    {
        RepairStatus.PendingCheckIn => "bg-secondary",
        RepairStatus.InspectionDiagnostic => "bg-info text-dark",
        RepairStatus.InProgress => "bg-primary",
        RepairStatus.WaitingParts => "bg-warning text-dark",
        RepairStatus.QualityCheck => "bg-purple text-white",
        RepairStatus.ReadyForDelivery => "bg-teal text-white",
        RepairStatus.Delivered => "bg-success",
        _ => "bg-secondary"
    };
}
