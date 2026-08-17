using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Common;

namespace CompanyCalendar.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public string? TaxNumber { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

}
