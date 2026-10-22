using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.net.Data;
using web.net.Models;

namespace web.net.Controllers;

public class DashboardController : Controller
{
    private readonly AppDbContext _context;
    private readonly DatabaseStatusInfo _dbInfo;

    public DashboardController(AppDbContext context, DatabaseStatusInfo dbInfo)
    {
        _context = context;
        _dbInfo = dbInfo;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var totalVehicles = await _context.Vehicles.CountAsync();
        var todayAppointments = await _context.ServiceAppointments
            .Where(a => a.ScheduledDate.Date == today)
            .CountAsync();

        var activeRepairs = await _context.RepairJobCards
            .Where(r => r.Status != RepairStatus.Delivered)
            .CountAsync();

        var totalRevenue = await _context.Invoices
            .Where(i => i.Status == InvoiceStatus.Paid || i.Status == InvoiceStatus.PartiallyPaid)
            .SumAsync(i => i.AmountPaid);

        var pendingReceivables = await _context.Invoices
            .Where(i => i.Status == InvoiceStatus.Unpaid || i.Status == InvoiceStatus.PartiallyPaid)
            .SumAsync(i => i.BalanceDue);

        var recentAppointments = await _context.ServiceAppointments
            .Include(a => a.Vehicle)
            .Include(a => a.Mechanic)
            .OrderByDescending(a => a.ScheduledDate)
            .Take(5)
            .ToListAsync();

        var activeJobs = await _context.RepairJobCards
            .Include(r => r.Vehicle)
            .Include(r => r.LeadMechanic)
            .Include(r => r.LaborItems)
            .Include(r => r.PartItems)
            .Where(r => r.Status != RepairStatus.Delivered)
            .OrderByDescending(r => r.StartedAt)
            .Take(5)
            .ToListAsync();

        var mechanics = await _context.Mechanics
            .Include(m => m.LeadJobs.Where(j => j.Status != RepairStatus.Delivered))
            .ToListAsync();

        ViewBag.TotalVehicles = totalVehicles;
        ViewBag.TodayAppointments = todayAppointments;
        ViewBag.ActiveRepairs = activeRepairs;
        ViewBag.TotalRevenue = totalRevenue;
        ViewBag.PendingReceivables = pendingReceivables;
        ViewBag.RecentAppointments = recentAppointments;
        ViewBag.ActiveJobs = activeJobs;
        ViewBag.Mechanics = mechanics;
        ViewBag.DatabaseProvider = _dbInfo.Provider;
        ViewBag.ConnectionString = _dbInfo.ConnectionString;

        return View();
    }
}
