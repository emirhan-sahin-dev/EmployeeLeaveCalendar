using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyCalendar.Web.ViewModels.LeaveRequests;

public class LeaveRequestViewModel
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Çalışan seçiniz.")]
    [Display(Name = "Çalışan")]
    public int EmployeeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "İzin türü seçiniz.")]
    [Display(Name = "İzin Türü")]
    public int LeaveTypeId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Başlangıç Tarihi")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Bitiş Tarihi")]
    public DateTime EndDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    public List<SelectListItem> Employees { get; set; } = new();

    public List<SelectListItem> LeaveTypes { get; set; } = new();
}