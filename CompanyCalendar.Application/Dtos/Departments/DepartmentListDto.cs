using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyCalendar.Application.DTOs.Departments;

public class DepartmentListDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int PositionCount { get; set; }

    public DateTime CreatedAt { get; set; }
}