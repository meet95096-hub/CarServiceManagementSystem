using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.net.Data;
using web.net.Models;

namespace web.net.Controllers;

public class VehiclesController : Controller
{
    private readonly AppDbContext _context;

    public VehiclesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Vehicles
    public async Task<IActionResult> Index(string? search, string? make, string? fuelType)
    {
        var query = _context.Vehicles
            .Include(v => v.RepairJobs)
            .Include(v => v.Appointments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(v =>
                v.LicensePlate.ToLower().Contains(search) ||
                v.Make.ToLower().Contains(search) ||
                v.Model.ToLower().Contains(search) ||
                v.OwnerName.ToLower().Contains(search) ||
                v.OwnerPhone.ToLower().Contains(search) ||
                (v.Vin != null && v.Vin.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(make))
        {
            query = query.Where(v => v.Make == make);
        }

        if (!string.IsNullOrWhiteSpace(fuelType))
        {
            query = query.Where(v => v.FuelType == fuelType);
        }

        var vehicles = await query.OrderByDescending(v => v.RegisteredAt).ToListAsync();

        ViewBag.Search = search;
        ViewBag.SelectedMake = make;
        ViewBag.SelectedFuel = fuelType;
        ViewBag.Makes = await _context.Vehicles.Select(v => v.Make).Distinct().ToListAsync();

        return View(vehicles);
    }

    // GET: /Vehicles/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vehicle = await _context.Vehicles
            .Include(v => v.Appointments)
                .ThenInclude(a => a.Mechanic)
            .Include(v => v.RepairJobs)
                .ThenInclude(r => r.LeadMechanic)
            .Include(v => v.RepairJobs)
                .ThenInclude(r => r.LaborItems)
            .Include(v => v.RepairJobs)
                .ThenInclude(r => r.PartItems)
            .Include(v => v.Invoices)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null) return NotFound();

        return View(vehicle);
    }

    // GET: /Vehicles/Create
    public IActionResult Create()
    {
        return View(new Vehicle { Year = DateTime.Now.Year });
    }

    // POST: /Vehicles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        // Check for duplicate license plate
        var existing = await _context.Vehicles
            .AnyAsync(v => v.LicensePlate.ToLower() == vehicle.LicensePlate.Trim().ToLower());

        if (existing)
        {
            ModelState.AddModelError("LicensePlate", "A vehicle with this license plate is already registered.");
        }

        if (ModelState.IsValid)
        {
            vehicle.LicensePlate = vehicle.LicensePlate.Trim().ToUpper();
            vehicle.RegisteredAt = DateTime.UtcNow;
            _context.Add(vehicle);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Vehicle '{vehicle.LicensePlate}' ({vehicle.Make} {vehicle.Model}) registered successfully!";
            return RedirectToAction(nameof(Details), new { id = vehicle.Id });
        }

        return View(vehicle);
    }

    // GET: /Vehicles/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound();

        return View(vehicle);
    }

    // POST: /Vehicles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id) return NotFound();

        var plateCheck = await _context.Vehicles
            .AnyAsync(v => v.Id != id && v.LicensePlate.ToLower() == vehicle.LicensePlate.Trim().ToLower());

        if (plateCheck)
        {
            ModelState.AddModelError("LicensePlate", "Another vehicle with this license plate already exists.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                vehicle.LicensePlate = vehicle.LicensePlate.Trim().ToUpper();
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Vehicle details updated successfully!";
                return RedirectToAction(nameof(Details), new { id = vehicle.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Vehicles.AnyAsync(v => v.Id == vehicle.Id))
                    return NotFound();
                throw;
            }
        }

        return View(vehicle);
    }

    // POST: /Vehicles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Vehicle '{vehicle.LicensePlate}' removed successfully.";
        }
        return RedirectToAction(nameof(Index));
    }
}
