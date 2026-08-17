using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CompanyCalendar.Application.DTOs.Positions;

public class UpdatePositionDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Pozisyon adı zorunludur.")]
    [StringLength(150)]
    [Display(Name = "Pozisyon Adı")]
    public string Name { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "Pozisyon Kodu")]
    public string? Code { get; set; }

    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Departman seçiniz.")]
    [Display(Name = "Departman")]
    public int DepartmentId { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; }
}
