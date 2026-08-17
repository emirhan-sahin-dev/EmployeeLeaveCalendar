using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Common;

namespace CompanyCalendar.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Position> Positions { get; set; }
        = new List<Position>();
    public ICollection<Employee> Employees { get; set; }
    = new List<Employee>();
}