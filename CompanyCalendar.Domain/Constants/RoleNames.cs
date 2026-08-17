using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyCalendar.Domain.Constants;

public static class RoleNames
{
    public const string SystemAdmin = "SystemAdmin";
    public const string HumanResources = "HumanResources";
    public const string DepartmentManager = "DepartmentManager";
    public const string Employee = "Employee";
    public const string ReadOnly = "ReadOnly";

    public static readonly string[] All =
    [
        SystemAdmin,
        HumanResources,
        DepartmentManager,
        Employee,
        ReadOnly
    ];
}
