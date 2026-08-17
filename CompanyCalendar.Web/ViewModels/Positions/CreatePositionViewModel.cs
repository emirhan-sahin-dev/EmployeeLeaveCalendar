using CompanyCalendar.Application.DTOs.Positions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyCalendar.Web.ViewModels.Positions;

public class CreatePositionViewModel
{
    public CreatePositionDto Position { get; set; } = new();

    public List<SelectListItem> Departments { get; set; } = new();
}