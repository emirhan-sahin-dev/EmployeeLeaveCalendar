using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CompanyCalendar.Application.DTOs.Departments;
using CompanyCalendar.Application.Interfaces.Services;
using CompanyCalendar.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace CompanyCalendar.Web.Controllers;

[Authorize(
    Roles =
        RoleNames.SystemAdmin + "," +
        RoleNames.HumanResources)]
public class DepartmentController : Controller
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(
        IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(
            cancellationToken);

        return View(departments);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(
            id,
            cancellationToken);

        if (department is null)
        {
            return NotFound();
        }

        return View(department);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateDepartmentDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateDepartmentDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _departmentService.CreateAsync(
            model,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Message);

            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetForUpdateAsync(
            id,
            cancellationToken);

        if (department is null)
        {
            return NotFound();
        }

        return View(department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdateDepartmentDto model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _departmentService.UpdateAsync(
            model,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Message);

            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.SystemAdmin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _departmentService.DeleteAsync(
            id,
            GetCurrentUserId(),
            cancellationToken);

        TempData[result.Success
            ? "SuccessMessage"
            : "ErrorMessage"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }
}
