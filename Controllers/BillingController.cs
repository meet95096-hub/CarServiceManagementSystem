using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.net.Data;
using web.net.Models;

namespace web.net.Controllers;

public class BillingController : Controller
{
    private readonly AppDbContext _context;

    public BillingController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Billing
    public async Task<IActionResult> Index(InvoiceStatus? status, string? search)
    {
        var query = _context.Invoices
            .Include(i => i.Vehicle)
            .Include(i => i.RepairJobCard)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(i =>
                i.InvoiceNumber.ToLower().Contains(search) ||
                i.Vehicle!.LicensePlate.ToLower().Contains(search) ||
                i.Vehicle.OwnerName.ToLower().Contains(search) ||
                i.RepairJobCard!.JobCardNumber.ToLower().Contains(search));
        }

        var invoices = await query.OrderByDescending(i => i.InvoiceDate).ToListAsync();

        ViewBag.SelectedStatus = status;
        ViewBag.Search = search;

        ViewBag.TotalBilled = await _context.Invoices.SumAsync(i => i.GrandTotal);
        ViewBag.TotalCollected = await _context.Invoices.SumAsync(i => i.AmountPaid);
        ViewBag.TotalUnpaid = await _context.Invoices.Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled).SumAsync(i => i.BalanceDue);
        ViewBag.UnpaidCount = await _context.Invoices.CountAsync(i => i.Status == InvoiceStatus.Unpaid);

        return View(invoices);
    }

    // GET: /Billing/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var invoice = await _context.Invoices
            .Include(i => i.Vehicle)
            .Include(i => i.RepairJobCard)
                .ThenInclude(r => r!.LeadMechanic)
            .Include(i => i.RepairJobCard)
                .ThenInclude(r => r!.LaborItems)
            .Include(i => i.RepairJobCard)
                .ThenInclude(r => r!.PartItems)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return NotFound();

        return View(invoice);
    }

    // GET: /Billing/Print/5
    public async Task<IActionResult> Print(int? id)
    {
        if (id == null) return NotFound();

        var invoice = await _context.Invoices
            .Include(i => i.Vehicle)
            .Include(i => i.RepairJobCard)
                .ThenInclude(r => r!.LeadMechanic)
            .Include(i => i.RepairJobCard)
                .ThenInclude(r => r!.LaborItems)
            .Include(i => i.RepairJobCard)
                .ThenInclude(r => r!.PartItems)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return NotFound();

        return View(invoice);
    }

    // POST: /Billing/GenerateFromJob
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateFromJob(int repairJobCardId, decimal discountPercentage = 0m, decimal taxRate = 10m)
    {
        var job = await _context.RepairJobCards
            .Include(j => j.Vehicle)
            .Include(j => j.LaborItems)
            .Include(j => j.PartItems)
            .Include(j => j.Invoice)
            .FirstOrDefaultAsync(j => j.Id == repairJobCardId);

        if (job == null) return NotFound();

        // If an invoice already exists for this job, redirect to it
        if (job.Invoice != null)
        {
            return RedirectToAction(nameof(Details), new { id = job.Invoice.Id });
        }

        // Calculate totals
        decimal laborTotal = job.LaborItems.Sum(l => l.Hours * l.HourlyRate);
        decimal partsTotal = job.PartItems.Sum(p => p.Quantity * p.UnitPrice);
        decimal subTotal = laborTotal + partsTotal;

        decimal discountAmount = Math.Round(subTotal * (discountPercentage / 100m), 2);
        decimal taxableAmount = subTotal - discountAmount;
        decimal taxAmount = Math.Round(taxableAmount * (taxRate / 100m), 2);
        decimal grandTotal = taxableAmount + taxAmount;

        var invoice = new Invoice
        {
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyMMddHHmm}",
            RepairJobCardId = job.Id,
            VehicleId = job.VehicleId,
            InvoiceDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            LaborTotal = laborTotal,
            PartsTotal = partsTotal,
            SubTotal = subTotal,
            DiscountPercentage = discountPercentage,
            DiscountAmount = discountAmount,
            TaxRate = taxRate,
            TaxAmount = taxAmount,
            GrandTotal = grandTotal,
            AmountPaid = 0m,
            BalanceDue = grandTotal,
            Status = InvoiceStatus.Unpaid,
            Notes = "All parts & mechanical labor are guaranteed under our 90-day / 5,000 km warranty."
        };

        _context.Invoices.Add(invoice);

        // Advance repair status if needed
        if (job.Status < RepairStatus.ReadyForDelivery)
        {
            job.Status = RepairStatus.ReadyForDelivery;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Bill / Invoice '{invoice.InvoiceNumber}' generated successfully!";
        return RedirectToAction(nameof(Details), new { id = invoice.Id });
    }

    // POST: /Billing/RecordPayment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordPayment(int id, decimal amount, string paymentMethod, string? transactionReference)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null) return NotFound();

        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Payment amount must be greater than zero.";
            return RedirectToAction(nameof(Details), new { id });
        }

        invoice.AmountPaid += amount;
        invoice.BalanceDue = Math.Max(0m, invoice.GrandTotal - invoice.AmountPaid);
        invoice.PaymentMethod = paymentMethod;
        invoice.TransactionReference = transactionReference ?? $"TXN-{DateTime.UtcNow:yyMMddHHmm}";
        invoice.PaidAt = DateTime.UtcNow;

        if (invoice.BalanceDue <= 0)
        {
            invoice.Status = InvoiceStatus.Paid;
        }
        else
        {
            invoice.Status = InvoiceStatus.PartiallyPaid;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Payment of ${amount:F2} recorded successfully! Status: {invoice.Status}";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /Billing/Recalculate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Recalculate(int id, decimal discountPercentage, decimal taxRate, string? notes)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null) return NotFound();

        invoice.DiscountPercentage = discountPercentage;
        invoice.TaxRate = taxRate;
        invoice.Notes = notes;

        invoice.DiscountAmount = Math.Round(invoice.SubTotal * (discountPercentage / 100m), 2);
        decimal taxableAmount = invoice.SubTotal - invoice.DiscountAmount;
        invoice.TaxAmount = Math.Round(taxableAmount * (taxRate / 100m), 2);
        invoice.GrandTotal = taxableAmount + invoice.TaxAmount;
        invoice.BalanceDue = Math.Max(0m, invoice.GrandTotal - invoice.AmountPaid);

        if (invoice.AmountPaid >= invoice.GrandTotal)
        {
            invoice.Status = InvoiceStatus.Paid;
        }
        else if (invoice.AmountPaid > 0)
        {
            invoice.Status = InvoiceStatus.PartiallyPaid;
        }
        else
        {
            invoice.Status = InvoiceStatus.Unpaid;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Invoice recalculated and updated successfully!";
        return RedirectToAction(nameof(Details), new { id });
    }
}
