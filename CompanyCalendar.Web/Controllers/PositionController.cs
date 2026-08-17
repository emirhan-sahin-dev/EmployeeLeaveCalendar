using System.Security.Claims;
using CompanyCalendar.Application.DTOs.Positions;
using CompanyCalendar.Application.Interfaces.Repositories;
using CompanyCalendar.Application.Interfaces.Services;
using CompanyCalendar.Domain.Constants;
using CompanyCalendar.Web.ViewModels.Positions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyCalendar.Web.Controllers;

[Authorize(
    Roles =
        RoleNames.SystemAdmin + "," +
        RoleNames.HumanResources)]
public class PositionController : Controller
{
    private readonly IPositionService _positionService;
    private readonly IDepartmentRepository _departmentRepository;

    public PositionController(
        IPositionService positionService,
        IDepartmentRepository departmentRepository)
    {
        _positionService = positionService;
        _departmentRepository = departmentRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var positions = await _positionService.GetAllAsync(
            cancellationToken);

        return View(positions);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        var position = await _positionService.GetByIdAsync(
            id,
            cancellationToken);

        if (position is null)
        {
            return NotFound();
        }

        return View(position);
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        var model = new CreatePositionViewModel
        {
            Departments =
                await GetDepartmentSelectListAsync(
                    cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreatePositionViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Departments =
                await GetDepartmentSelectListAsync(
                    cancellationToken);

            return View(model);
        }

        var result = await _positionService.CreateAsync(
            model.Position,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Message);

            model.Departments =
                await GetDepartmentSelectListAsync(
                    cancellationToken);

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
        var position = await _positionService.GetForUpdateAsync(
            id,
            cancellationToken);

        if (position is null)
        {
            return NotFound();
        }

        var model = new UpdatePositionViewModel
        {
            Position = position,
            Departments =
                await GetDepartmentSelectListAsync(
                    cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdatePositionViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Departments =
                await GetDepartmentSelectListAsync(
                    cancellationToken);

            return View(model);
        }

        var result = await _positionService.UpdateAsync(
            model.Position,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Message);

            model.Departments =
                await GetDepartmentSelectListAsync(
                    cancellationToken);

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
        var result = await _positionService.DeleteAsync(
            id,
            GetCurrentUserId(),
            cancellationToken);

        TempData[result.Success
            ? "SuccessMessage"
            : "ErrorMessage"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>>
        GetDepartmentSelectListAsync(
            CancellationToken cancellationToken)
    {
        var departments =
            await _departmentRepository.GetActiveAsync(
                cancellationToken);

        return departments
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }
}
