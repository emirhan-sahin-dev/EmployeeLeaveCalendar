using System.ComponentModel.DataAnnotations;

namespace CompanyCalendar.Web.ViewModels.LeaveTypes;

public class LeaveTypeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "İzin türü adı zorunludur.")]
    [Display(Name = "İzin Türü")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Takvim Rengi")]
    public string Color { get; set; } = "#0d6efd";

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
}