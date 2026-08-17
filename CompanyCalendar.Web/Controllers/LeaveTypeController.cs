using CompanyCalendar.Domain.Constants;
using CompanyCalendar.Domain.Entities;
using CompanyCalendar.Infrastructure.Data;
using CompanyCalendar.Web.ViewModels.LeaveTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyCalendar.Web.Controllers;

[Authorize(
    Roles =
        RoleNames.SystemAdmin + "," +
        RoleNames.HumanResources)]
public class LeaveTypeController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaveTypeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var leaveTypes = await _context.LeaveTypes
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return View(leaveTypes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new LeaveTypeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        LeaveTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var name = model.Name.Trim();

        var exists = await _context.LeaveTypes
            .AnyAsync(x => x.Name == name);

        if (exists)
        {
            ModelState.AddModelError(
                nameof(model.Name),
                "Bu izin türü zaten mevcut.");

            return View(model);
        }

        var leaveType = new LeaveType
        {
            Name = name,
            Color = model.Color,
            IsActive = model.IsActive
        };

        _context.LeaveTypes.Add(leaveType);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "İzin türü başarıyla eklendi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var leaveType = await _context.LeaveTypes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (leaveType is null)
        {
            return NotFound();
        }

        return View(new LeaveTypeViewModel
        {
            Id = leaveType.Id,
            Name = leaveType.Name,
            Color = leaveType.Color,
            IsActive = leaveType.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        LeaveTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var leaveType = await _context.LeaveTypes
            .FirstOrDefaultAsync(x => x.Id == model.Id);

        if (leaveType is null)
        {
            return NotFound();
        }

        var name = model.Name.Trim();

        var exists = await _context.LeaveTypes
            .AnyAsync(x =>
                x.Name == name &&
                x.Id != model.Id);

        if (exists)
        {
            ModelState.AddModelError(
                nameof(model.Name),
                "Bu izin türü başka bir kayıtta mevcut.");

            return View(model);
        }

        leaveType.Name = name;
        leaveType.Color = model.Color;
        leaveType.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "İzin türü güncellendi.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.SystemAdmin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var leaveType = await _context.LeaveTypes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (leaveType is null)
        {
            return NotFound();
        }

        leaveType.IsDeleted = true;
        leaveType.IsActive = false;
        leaveType.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "İzin türü kaldırıldı.";

        return RedirectToAction(nameof(Index));
    }
}