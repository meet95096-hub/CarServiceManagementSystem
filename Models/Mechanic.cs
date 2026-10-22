using System.ComponentModel.DataAnnotations;

namespace web.net.Models;

public class Mechanic
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Specialization { get; set; } = "Master Automotive Technician";

    [StringLength(25)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(50)]
    [Display(Name = "Assigned Bay")]
    public string AssignedBay { get; set; } = "Bay 1";

    public bool IsAvailable { get; set; } = true;

    public int ExperienceYears { get; set; } = 5;

    public string AvatarInitials => string.Join("", FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s[0])).ToUpper();

    public ICollection<ServiceAppointment> Appointments { get; set; } = new List<ServiceAppointment>();
    public ICollection<RepairJobCard> LeadJobs { get; set; } = new List<RepairJobCard>();
}
