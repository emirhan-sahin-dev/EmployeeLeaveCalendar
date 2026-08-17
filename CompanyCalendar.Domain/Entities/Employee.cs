using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Common;

namespace CompanyCalendar.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<LeaveRequest> LeaveRequests { get; set; }
    = new List<LeaveRequest>();
}
