using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web.net.Data;
using web.net.Models;

namespace web.net.Controllers;

public class RepairTrackingController : Controller
{
    private readonly AppDbContext _context;

    public RepairTrackingController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /RepairTracking
    public async Task<IActionResult> Index(RepairStatus? status, string? search)
    {
        var query = _context.RepairJobCards
            .Include(r => r.Vehicle)
            .Include(r => r.LeadMechanic)
            .Include(r => r.LaborItems)
            .Include(r => r.PartItems)
            .Include(r => r.Invoice)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(r =>
                r.JobCardNumber.ToLower().Contains(search) ||
                r.Vehicle!.LicensePlate.ToLower().Contains(search) ||
                r.Vehicle.Make.ToLower().Contains(search) ||
                r.Vehicle.Model.ToLower().Contains(search) ||
                r.Vehicle.OwnerName.ToLower().Contains(search));
        }

        var jobs = await query.OrderByDescending(r => r.StartedAt).ToListAsync();

        ViewBag.SelectedStatus = status;
        ViewBag.Search = search;

        // Group counts for quick filter pills
        ViewBag.PendingCount = await _context.RepairJobCards.CountAsync(r => r.Status == RepairStatus.PendingCheckIn);
        ViewBag.DiagnosticCount = await _context.RepairJobCards.CountAsync(r => r.Status == RepairStatus.InspectionDiagnostic);
        ViewBag.InProgressCount = await _context.RepairJobCards.CountAsync(r => r.Status == RepairStatus.InProgress);
        ViewBag.WaitingPartsCount = await _context.RepairJobCards.CountAsync(r => r.Status == RepairStatus.WaitingParts);
        ViewBag.QualityCheckCount = await _context.RepairJobCards.CountAsync(r => r.Status == RepairStatus.QualityCheck);
        ViewBag.ReadyCount = await _context.RepairJobCards.CountAsync(r => r.Status == RepairStatus.ReadyForDelivery);
        ViewBag.DeliveredCount = await _context.RepairJobCards.CountAsync(r => r.Status == RepairStatus.Delivered);

        return View(jobs);
    }

    // GET: /RepairTracking/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var job = await _context.RepairJobCards
            .Include(r => r.Vehicle)
            .Include(r => r.LeadMechanic)
            .Include(r => r.LaborItems)
            .Include(r => r.PartItems)
            .Include(r => r.Invoice)
            .Include(r => r.ServiceAppointment)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (job == null) return NotFound();

        return View(job);
    }

    // GET: /RepairTracking/Create
    public async Task<IActionResult> Create(int? vehicleId)
    {
        await PopulateDropdownsAsync(vehicleId);

        var model = new RepairJobCard
        {
            JobCardNumber = $"JOB-{DateTime.UtcNow:yyMMddHHmm}",
            StartedAt = DateTime.UtcNow,
            EstimatedCompletionAt = DateTime.UtcNow.AddHours(4),
            Bay = "Bay 1 - Diagnostics & Engine"
        };

        if (vehicleId.HasValue)
        {
            model.VehicleId = vehicleId.Value;
            var v = await _context.Vehicles.FindAsync(vehicleId.Value);
            if (v != null)
            {
                model.OdometerIn = v.CurrentMileage;
            }
        }

        return View(model);
    }

    // POST: /RepairTracking/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairJobCard jobCard)
    {
        if (ModelState.IsValid)
        {
            jobCard.JobCardNumber = jobCard.JobCardNumber.Trim().ToUpper();
            jobCard.StartedAt = DateTime.UtcNow;

            _context.RepairJobCards.Add(jobCard);
            await _context.SaveChangesAsync();

            // Update vehicle current mileage if entered higher
            var vehicle = await _context.Vehicles.FindAsync(jobCard.VehicleId);
            if (vehicle != null && jobCard.OdometerIn > vehicle.CurrentMileage)
            {
                vehicle.CurrentMileage = jobCard.OdometerIn;
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Repair Job Card '{jobCard.JobCardNumber}' opened successfully!";
            return RedirectToAction(nameof(Details), new { id = jobCard.Id });
        }

        await PopulateDropdownsAsync(jobCard.VehicleId);
        return View(jobCard);
    }

    // POST: /RepairTracking/AdvanceStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdvanceStatus(int id)
    {
        var job = await _context.RepairJobCards.FindAsync(id);
        if (job == null) return NotFound();

        if (job.Status < RepairStatus.Delivered)
        {
            job.Status = (RepairStatus)((int)job.Status + 1);

            if (job.Status == RepairStatus.Delivered)
            {
                job.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Repair status advanced to '{job.Status}'!";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /RepairTracking/SetStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, RepairStatus status)
    {
        var job = await _context.RepairJobCards.FindAsync(id);
        if (job == null) return NotFound();

        job.Status = status;
        if (status == RepairStatus.Delivered && !job.CompletedAt.HasValue)
        {
            job.CompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Repair status updated to '{status}'!";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /RepairTracking/UpdateInspection
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInspection(int id, bool checkEngineOil, bool checkBrakeSystem,
        bool checkBatteryHealth, bool checkTirePressureTread, bool checkCoolantRadiator,
        bool checkSuspensionSteering, bool checkLightsElectronics, string overallInspectionGrade)
    {
        var job = await _context.RepairJobCards.FindAsync(id);
        if (job == null) return NotFound();

        job.CheckEngineOil = checkEngineOil;
        job.CheckBrakeSystem = checkBrakeSystem;
        job.CheckBatteryHealth = checkBatteryHealth;
        job.CheckTirePressureTread = checkTirePressureTread;
        job.CheckCoolantRadiator = checkCoolantRadiator;
        job.CheckSuspensionSteering = checkSuspensionSteering;
        job.CheckLightsElectronics = checkLightsElectronics;
        job.OverallInspectionGrade = overallInspectionGrade;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Inspection checklist saved successfully!";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /RepairTracking/UpdateNotes
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateNotes(int id, string? diagnosticNotes, string? workPerformedSummary)
    {
        var job = await _context.RepairJobCards.FindAsync(id);
        if (job == null) return NotFound();

        job.DiagnosticNotes = diagnosticNotes;
        job.WorkPerformedSummary = workPerformedSummary;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Technician diagnosis and notes updated!";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /RepairTracking/AddLabor
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLabor(int repairJobCardId, string description, decimal hours, decimal hourlyRate)
    {
        if (!string.IsNullOrWhiteSpace(description) && hours > 0 && hourlyRate >= 0)
        {
            var item = new RepairLaborItem
            {
                RepairJobCardId = repairJobCardId,
                Description = description.Trim(),
                Hours = hours,
                HourlyRate = hourlyRate
            };

            _context.RepairLaborItems.Add(item);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Labor charge item added!";
        }

        return RedirectToAction(nameof(Details), new { id = repairJobCardId });
    }

    // POST: /RepairTracking/DeleteLabor
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLabor(int id, int repairJobCardId)
    {
        var item = await _context.RepairLaborItems.FindAsync(id);
        if (item != null)
        {
            _context.RepairLaborItems.Remove(item);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Labor item removed!";
        }

        return RedirectToAction(nameof(Details), new { id = repairJobCardId });
    }

    // POST: /RepairTracking/AddPart
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPart(int repairJobCardId, string partName, string? partNumber, int quantity, decimal unitPrice)
    {
        if (!string.IsNullOrWhiteSpace(partName) && quantity > 0 && unitPrice >= 0)
        {
            var item = new RepairPartItem
            {
                RepairJobCardId = repairJobCardId,
                PartName = partName.Trim(),
                PartNumber = partNumber?.Trim(),
                Quantity = quantity,
                UnitPrice = unitPrice
            };

            _context.RepairPartItems.Add(item);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Replacement part added!";
        }

        return RedirectToAction(nameof(Details), new { id = repairJobCardId });
    }

    // POST: /RepairTracking/DeletePart
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePart(int id, int repairJobCardId)
    {
        var item = await _context.RepairPartItems.FindAsync(id);
        if (item != null)
        {
            _context.RepairPartItems.Remove(item);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Part item removed!";
        }

        return RedirectToAction(nameof(Details), new { id = repairJobCardId });
    }

    // GET: /RepairTracking/Live
    // Customer-facing live tracking portal
    public async Task<IActionResult> Live(string? q)
    {
        RepairJobCard? job = null;

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim().ToLower();
            job = await _context.RepairJobCards
                .Include(r => r.Vehicle)
                .Include(r => r.LeadMechanic)
                .Include(r => r.LaborItems)
                .Include(r => r.PartItems)
                .Include(r => r.Invoice)
                .FirstOrDefaultAsync(r =>
                    r.JobCardNumber.ToLower() == q ||
                    r.Vehicle!.LicensePlate.ToLower() == q);

            if (job == null)
            {
                ViewBag.NotFoundMessage = $"No active service record found for '{q}'. Please double check your Job Card # or License Plate.";
            }
        }

        ViewBag.Query = q;
        return View(job);
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

        ViewBag.VehiclesList = new SelectList(vehicles, "Id", "Display", selectedVehicleId);
        ViewBag.MechanicsList = new SelectList(mechanics, "Id", "Display");
    }
}
