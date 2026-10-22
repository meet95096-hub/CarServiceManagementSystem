using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web.net.Data;
using web.net.Models;

namespace web.net.Controllers;

public class AppointmentsController : Controller
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Appointments
    public async Task<IActionResult> Index(AppointmentStatus? status, DateTime? date, string? search)
    {
        var query = _context.ServiceAppointments
            .Include(a => a.Vehicle)
            .Include(a => a.Mechanic)
            .Include(a => a.ServicePackage)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (date.HasValue)
        {
            query = query.Where(a => a.ScheduledDate.Date == date.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(a =>
                a.AppointmentCode.ToLower().Contains(search) ||
                a.ServiceTitle.ToLower().Contains(search) ||
                a.Vehicle!.LicensePlate.ToLower().Contains(search) ||
                a.Vehicle.OwnerName.ToLower().Contains(search));
        }

        var appointments = await query.OrderByDescending(a => a.ScheduledDate).ThenBy(a => a.TimeSlot).ToListAsync();

        ViewBag.SelectedStatus = status;
        ViewBag.SelectedDate = date?.ToString("yyyy-MM-dd");
        ViewBag.Search = search;

        return View(appointments);
    }

    // GET: /Appointments/Create
    public async Task<IActionResult> Create(int? vehicleId)
    {
        await PopulateDropdownsAsync(vehicleId);

        var model = new ServiceAppointment
        {
            AppointmentCode = $"APT-{DateTime.UtcNow:yyMMddHHmm}",
            ScheduledDate = DateTime.Today,
            TimeSlot = "09:00 AM - 11:00 AM",
            Bay = "Bay 1 - Diagnostics & Engine"
        };

        if (vehicleId.HasValue)
        {
            model.VehicleId = vehicleId.Value;
        }

        return View(model);
    }

    // POST: /Appointments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceAppointment appointment)
    {
        if (ModelState.IsValid)
        {
            appointment.AppointmentCode = appointment.AppointmentCode.Trim().ToUpper();
            appointment.CreatedAt = DateTime.UtcNow;

            // If a service package was selected and estimated cost is 0, populate with package price
            if (appointment.ServicePackageId.HasValue && appointment.EstimatedCost == 0)
            {
                var package = await _context.ServicePackages.FindAsync(appointment.ServicePackageId.Value);
                if (package != null)
                {
                    appointment.EstimatedCost = package.BaseLaborPrice;
                }
            }

            _context.Add(appointment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Appointment '{appointment.AppointmentCode}' booked successfully!";
            return RedirectToAction(nameof(Details), new { id = appointment.Id });
        }

        await PopulateDropdownsAsync(appointment.VehicleId);
        return View(appointment);
    }

    // GET: /Appointments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var appointment = await _context.ServiceAppointments
            .Include(a => a.Vehicle)
            .Include(a => a.Mechanic)
            .Include(a => a.ServicePackage)
            .Include(a => a.LinkedRepairJob)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null) return NotFound();

        return View(appointment);
    }

    // POST: /Appointments/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, AppointmentStatus status)
    {
        var appt = await _context.ServiceAppointments.FindAsync(id);
        if (appt != null)
        {
            appt.Status = status;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Appointment status updated to '{status}'!";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /Appointments/ConvertToJobCard
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConvertToJobCard(int id)
    {
        var appt = await _context.ServiceAppointments
            .Include(a => a.Vehicle)
            .Include(a => a.ServicePackage)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appt == null) return NotFound();

        if (appt.LinkedRepairJobId.HasValue)
        {
            // Already converted
            return RedirectToAction("Details", "RepairTracking", new { id = appt.LinkedRepairJobId.Value });
        }

        // Create new Job Card
        var jobCard = new RepairJobCard
        {
            JobCardNumber = $"JOB-{DateTime.UtcNow:yyMMddHHmm}",
            VehicleId = appt.VehicleId,
            ServiceAppointmentId = appt.Id,
            LeadMechanicId = appt.MechanicId,
            Bay = appt.Bay,
            OdometerIn = appt.Vehicle?.CurrentMileage ?? 0,
            DiagnosticNotes = string.IsNullOrWhiteSpace(appt.CustomerNotes) ? "Inspection initiated from booked appointment." : appt.CustomerNotes,
            WorkPerformedSummary = appt.ServiceTitle,
            Status = RepairStatus.InProgress,
            StartedAt = DateTime.UtcNow,
            EstimatedCompletionAt = DateTime.UtcNow.AddHours(3)
        };

        _context.RepairJobCards.Add(jobCard);
        await _context.SaveChangesAsync();

        // If appointment had a service package, add base labor item
        if (appt.ServicePackage != null)
        {
            var laborItem = new RepairLaborItem
            {
                RepairJobCardId = jobCard.Id,
                Description = appt.ServicePackage.Name,
                Hours = appt.ServicePackage.EstimatedHours,
                HourlyRate = appt.ServicePackage.EstimatedHours > 0 ? Math.Round(appt.ServicePackage.BaseLaborPrice / appt.ServicePackage.EstimatedHours, 2) : 85.00m
            };
            _context.RepairLaborItems.Add(laborItem);
        }

        appt.Status = AppointmentStatus.InService;
        appt.LinkedRepairJobId = jobCard.Id;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Service Job Card '{jobCard.JobCardNumber}' successfully generated from appointment!";
        return RedirectToAction("Details", "RepairTracking", new { id = jobCard.Id });
    }

    private async Task PopulateDropdownsAsync(int? selectedVehicleId = null)
    {
        var vehicles = await _context.Vehicles
            .OrderBy(v => v.Make)
            .ThenBy(v => v.Model)
            .Select(v => new
            {
                v.Id,
                Display = $"{v.LicensePlate} - {v.Make} {v.Model} ({v.OwnerName})"
            })
            .ToListAsync();

        var mechanics = await _context.Mechanics
            .Where(m => m.IsAvailable)
            .OrderBy(m => m.FullName)
            .Select(m => new
            {
                m.Id,
                Display = $"{m.FullName} ({m.Specialization})"
            })
            .ToListAsync();

        var packages = await _context.ServicePackages
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Id,
                Display = $"{p.Name} (${p.BaseLaborPrice:F2})"
            })
            .ToListAsync();

        ViewBag.VehiclesList = new SelectList(vehicles, "Id", "Display", selectedVehicleId);
        ViewBag.MechanicsList = new SelectList(mechanics, "Id", "Display");
        ViewBag.PackagesList = new SelectList(packages, "Id", "Display");
    }
}
