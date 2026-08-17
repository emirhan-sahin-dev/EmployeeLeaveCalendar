using CompanyCalendar.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyCalendar.Web.Controllers;

[Authorize(Roles = RoleNames.SystemAdmin)]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}