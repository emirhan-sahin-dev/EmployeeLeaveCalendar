using CompanyCalendar.Domain.Enums;
using CompanyCalendar.Infrastructure.Data;
using CompanyCalendar.Web.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyCalendar.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var totalActiveEmployees = await _context.Employees
            .AsNoTracking()
            .CountAsync(x => x.IsActive);

        var todayLeaves = await _context.LeaveRequests
            .AsNoTracking()
            .Include(x => x.Employee)
                .ThenInclude(x => x.Department)
            .Include(x => x.LeaveType)
            .Where(x =>
                x.Status == LeaveRequestStatus.Approved &&
                x.StartDate <= today &&
                x.EndDate >= today)
            .OrderBy(x => x.Employee.FirstName)
            .ThenBy(x => x.Employee.LastName)
            .ToListAsync();

        var pendingLeaveRequests = await _context.LeaveRequests
            .AsNoTracking()
            .CountAsync(x =>
                x.Status == LeaveRequestStatus.Pending);

        var employeesOnLeaveToday = todayLeaves
            .Select(x => x.EmployeeId)
            .Distinct()
            .Count();

        var model = new DashboardViewModel
        {
            TotalActiveEmployees = totalActiveEmployees,

            EmployeesOnLeaveToday = employeesOnLeaveToday,

            EmployeesWorkingToday =
                totalActiveEmployees - employeesOnLeaveToday,

            PendingLeaveRequests = pendingLeaveRequests,

            TodayLeaves = todayLeaves
        };

        return View(model);
    }
}