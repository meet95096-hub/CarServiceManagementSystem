using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.net.Data;
using web.net.Models;

namespace web.net.Controllers;

public class MechanicsController : Controller
{
    private readonly AppDbContext _context;

    public MechanicsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var mechanics = await _context.Mechanics
            .Include(m => m.LeadJobs.Where(j => j.Status != RepairStatus.Delivered))
                .ThenInclude(j => j.Vehicle)
            .Include(m => m.Appointments.Where(a => a.ScheduledDate.Date >= DateTime.Today))
            .ToListAsync();

        var packages = await _context.ServicePackages.ToListAsync();

        ViewBag.Packages = packages;
        return View(mechanics);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAvailability(int id)
    {
        var mechanic = await _context.Mechanics.FindAsync(id);
        if (mechanic != null)
        {
            mechanic.IsAvailable = !mechanic.IsAvailable;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Availability status for {mechanic.FullName} updated!";
        }
        return RedirectToAction(nameof(Index));
    }
}
