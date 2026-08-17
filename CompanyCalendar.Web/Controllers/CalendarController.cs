using CompanyCalendar.Domain.Constants;
using CompanyCalendar.Domain.Enums;
using CompanyCalendar.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyCalendar.Web.Controllers;

[Authorize(
    Roles =
        RoleNames.SystemAdmin + "," +
        RoleNames.HumanResources)]
public class CalendarController : Controller
{
    private readonly ApplicationDbContext _context;

    public CalendarController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.Departments = await _context.Departments
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();

        ViewBag.LeaveTypes = await _context.LeaveTypes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents(
    DateTime start,
    DateTime end,
    int? departmentId,
    int? leaveTypeId)
    {
        var query = _context.LeaveRequests
            .AsNoTracking()
            .Include(x => x.Employee)
                .ThenInclude(x => x.Department)
            .Include(x => x.LeaveType)
            .Where(x =>
                x.Status == LeaveRequestStatus.Approved &&
                x.StartDate < end &&
                x.EndDate >= start);

        if (departmentId.HasValue)
        {
            query = query.Where(x =>
                x.Employee.DepartmentId == departmentId.Value);
        }

        if (leaveTypeId.HasValue)
        {
            query = query.Where(x =>
                x.LeaveTypeId == leaveTypeId.Value);
        }

        var requests = await query.ToListAsync();

        var events = requests.Select(x => new
        {
            id = x.Id,

            title =
                x.Employee.FirstName + " " +
                x.Employee.LastName +
                " - " +
                x.LeaveType.Name,

            start = x.StartDate.ToString("yyyy-MM-dd"),

            end = x.EndDate
                .AddDays(1)
                .ToString("yyyy-MM-dd"),

            color = x.LeaveType.Color,

            extendedProps = new
            {
                employeeName =
                    x.Employee.FirstName + " " +
                    x.Employee.LastName,

                department =
                    x.Employee.Department.Name,

                leaveType =
                    x.LeaveType.Name,

                startDate =
                    x.StartDate.ToString("dd.MM.yyyy"),

                endDate =
                    x.EndDate.ToString("dd.MM.yyyy"),

                description =
                    x.Description ?? "-"
            }
        });

        return Json(events);
    }
}