using CompanyCalendar.Domain.Constants;
using CompanyCalendar.Domain.Entities;
using CompanyCalendar.Domain.Enums;
using CompanyCalendar.Infrastructure.Data;
using CompanyCalendar.Web.ViewModels.LeaveRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CompanyCalendar.Web.Controllers;

[Authorize(
    Roles =
        RoleNames.SystemAdmin + "," +
        RoleNames.HumanResources)]
public class LeaveRequestController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaveRequestController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? status)
    {
        var query = _context.LeaveRequests
            .AsNoTracking()
            .Include(x => x.Employee)
                .ThenInclude(x => x.Department)
            .Include(x => x.LeaveType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (status == "pending")
            {
                query = query.Where(x =>
                    x.Status == LeaveRequestStatus.Pending);
            }
            else if (status == "approved")
            {
                query = query.Where(x =>
                    x.Status == LeaveRequestStatus.Approved);
            }
            else if (status == "rejected")
            {
                query = query.Where(x =>
                    x.Status == LeaveRequestStatus.Rejected);
            }
        }

        var requests = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        ViewBag.Status = status;

        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new LeaveRequestViewModel();

        await LoadSelectionsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        LeaveRequestViewModel model)
    {
        if (model.EndDate < model.StartDate)
        {
            ModelState.AddModelError(
                nameof(model.EndDate),
                "Bitiş tarihi başlangıç tarihinden önce olamaz.");
        }

        if (!ModelState.IsValid)
        {
            await LoadSelectionsAsync(model);

            return View(model);
        }

        var employeeExists = await _context.Employees.AnyAsync(
            x => x.Id == model.EmployeeId && x.IsActive);

        if (!employeeExists)
        {
            ModelState.AddModelError(
                nameof(model.EmployeeId),
                "Seçilen çalışan bulunamadı.");

            await LoadSelectionsAsync(model);

            return View(model);
        }

        var leaveTypeExists = await _context.LeaveTypes.AnyAsync(
            x => x.Id == model.LeaveTypeId && x.IsActive);

        if (!leaveTypeExists)
        {
            ModelState.AddModelError(
                nameof(model.LeaveTypeId),
                "Seçilen izin türü bulunamadı.");

            await LoadSelectionsAsync(model);

            return View(model);
        }

        var overlapExists = await _context.LeaveRequests.AnyAsync(
            x =>
                x.EmployeeId == model.EmployeeId &&
                x.Status != LeaveRequestStatus.Rejected &&
                model.StartDate <= x.EndDate &&
                model.EndDate >= x.StartDate);

        if (overlapExists)
        {
            ModelState.AddModelError(
                string.Empty,
                "Bu çalışanın seçilen tarihlerde başka bir izin kaydı bulunmaktadır.");

            await LoadSelectionsAsync(model);

            return View(model);
        }

        var request = new LeaveRequest
        {
            EmployeeId = model.EmployeeId,
            LeaveTypeId = model.LeaveTypeId,
            StartDate = model.StartDate.Date,
            EndDate = model.EndDate.Date,
            Description = string.IsNullOrWhiteSpace(
                model.Description)
                ? null
                : model.Description.Trim(),
            Status = LeaveRequestStatus.Pending
        };

        _context.LeaveRequests.Add(request);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "İzin talebi oluşturuldu ve onay bekliyor.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var request = await _context.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id);

        if (request is null)
        {
            return NotFound();
        }

        if (request.Status != LeaveRequestStatus.Pending)
        {
            TempData["ErrorMessage"] =
                "Bu izin talebi daha önce sonuçlandırılmış.";

            return RedirectToAction(nameof(Index));
        }

        request.Status = LeaveRequestStatus.Approved;
        request.ApprovedAt = DateTime.UtcNow;
        request.RejectedAt = null;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "İzin talebi onaylandı.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var request = await _context.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id);

        if (request is null)
        {
            return NotFound();
        }

        if (request.Status != LeaveRequestStatus.Pending)
        {
            TempData["ErrorMessage"] =
                "Bu izin talebi daha önce sonuçlandırılmış.";

            return RedirectToAction(nameof(Index));
        }

        request.Status = LeaveRequestStatus.Rejected;
        request.RejectedAt = DateTime.UtcNow;
        request.ApprovedAt = null;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "İzin talebi reddedildi.";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSelectionsAsync(
        LeaveRequestViewModel model)
    {
        model.Employees = await _context.Employees
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FirstName + " " + x.LastName
            })
            .ToListAsync();

        model.LeaveTypes = await _context.LeaveTypes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToListAsync();
    }
}