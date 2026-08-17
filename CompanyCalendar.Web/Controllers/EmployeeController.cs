using CompanyCalendar.Domain.Constants;
using CompanyCalendar.Domain.Entities;
using CompanyCalendar.Infrastructure.Data;
using CompanyCalendar.Web.ViewModels.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CompanyCalendar.Web.Controllers;

[Authorize(
    Roles =
        RoleNames.SystemAdmin + "," +
        RoleNames.HumanResources)]
public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Employees
            .AsNoTracking()
            .Include(x => x.Department)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.FirstName.Contains(search) ||
                x.LastName.Contains(search) ||
                (x.Email != null &&
                 x.Email.Contains(search)) ||
                x.Department.Name.Contains(search));
        }

        var employees = await query
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync();

        ViewBag.Search = search;

        return View(employees);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new EmployeeFormViewModel();

        await LoadDepartmentsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        EmployeeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync(model);
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            var emailExists = await _context.Employees
                .AnyAsync(x => x.Email == model.Email.Trim());

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Bu e-posta adresi zaten kullanılıyor.");

                await LoadDepartmentsAsync(model);

                return View(model);
            }
        }

        var employee = new Employee
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = string.IsNullOrWhiteSpace(model.Email)
                ? null
                : model.Email.Trim().ToLowerInvariant(),
            DepartmentId = model.DepartmentId,
            IsActive = model.IsActive
        };

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Çalışan başarıyla eklendi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee is null)
        {
            return NotFound();
        }

        var model = new EmployeeFormViewModel
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            DepartmentId = employee.DepartmentId,
            IsActive = employee.IsActive
        };

        await LoadDepartmentsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        EmployeeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync(model);
            return View(model);
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(x => x.Id == model.Id);

        if (employee is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            var email = model.Email.Trim().ToLowerInvariant();

            var emailExists = await _context.Employees
                .AnyAsync(x =>
                    x.Email == email &&
                    x.Id != model.Id);

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Bu e-posta adresi başka bir çalışan tarafından kullanılıyor.");

                await LoadDepartmentsAsync(model);

                return View(model);
            }
        }

        employee.FirstName = model.FirstName.Trim();
        employee.LastName = model.LastName.Trim();
        employee.Email = string.IsNullOrWhiteSpace(model.Email)
            ? null
            : model.Email.Trim().ToLowerInvariant();

        employee.DepartmentId = model.DepartmentId;
        employee.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Çalışan bilgileri güncellendi.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.SystemAdmin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee is null)
        {
            return NotFound();
        }

        employee.IsDeleted = true;
        employee.IsActive = false;
        employee.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Çalışan kaldırıldı.";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDepartmentsAsync(
        EmployeeFormViewModel model)
    {
        model.Departments = await _context.Departments
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