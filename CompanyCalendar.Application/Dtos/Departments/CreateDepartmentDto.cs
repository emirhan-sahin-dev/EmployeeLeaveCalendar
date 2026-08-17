using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CompanyCalendar.Application.DTOs.Departments;

public class CreateDepartmentDto
{
    [Required(ErrorMessage = "Departman adı zorunludur.")]
    [StringLength(
        150,
        ErrorMessage = "Departman adı en fazla 150 karakter olabilir.")]
    [Display(Name = "Departman Adı")]
    public string Name { get; set; } = string.Empty;

    [StringLength(
        30,
        ErrorMessage = "Departman kodu en fazla 30 karakter olabilir.")]
    [Display(Name = "Departman Kodu")]
    public string? Code { get; set; }

    [StringLength(
        500,
        ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
}
