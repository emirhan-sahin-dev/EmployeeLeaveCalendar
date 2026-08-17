 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Common;

namespace CompanyCalendar.Domain.Entities;

public class Position : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;
}
