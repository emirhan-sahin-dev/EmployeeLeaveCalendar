using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Common;
using CompanyCalendar.Domain.Enums;

namespace CompanyCalendar.Domain.Entities;

public class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public int LeaveTypeId { get; set; }

    public LeaveType LeaveType { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Description { get; set; }

    public LeaveRequestStatus Status { get; set; }
        = LeaveRequestStatus.Pending;

    public DateTime? ApprovedAt { get; set; }

    public DateTime? RejectedAt { get; set; }
}
