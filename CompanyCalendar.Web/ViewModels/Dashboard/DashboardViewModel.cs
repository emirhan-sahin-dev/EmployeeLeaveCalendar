using CompanyCalendar.Domain.Entities;

namespace CompanyCalendar.Web.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalActiveEmployees { get; set; }

    public int EmployeesOnLeaveToday { get; set; }

    public int EmployeesWorkingToday { get; set; }

    public int PendingLeaveRequests { get; set; }

    public List<LeaveRequest> TodayLeaves { get; set; } = new();
}