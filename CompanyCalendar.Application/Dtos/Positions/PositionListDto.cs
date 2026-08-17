using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyCalendar.Application.DTOs.Positions;

public class PositionListDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
