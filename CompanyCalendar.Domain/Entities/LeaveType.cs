using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Common;

namespace CompanyCalendar.Domain.Entities;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Color { get; set; } = "#0d6efd";

    public bool IsActive { get; set; } = true;

    public ICollection<LeaveRequest> LeaveRequests { get; set; }
    = new List<LeaveRequest>();
}