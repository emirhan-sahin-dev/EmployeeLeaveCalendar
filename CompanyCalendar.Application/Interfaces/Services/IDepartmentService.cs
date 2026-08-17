using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Application.DTOs.Departments;

namespace CompanyCalendar.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<List<DepartmentListDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DepartmentDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<UpdateDepartmentDto?> GetForUpdateAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> CreateAsync(
        CreateDepartmentDto dto,
        string? userId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> UpdateAsync(
        UpdateDepartmentDto dto,
        string? userId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> DeleteAsync(
        int id,
        string? userId,
        CancellationToken cancellationToken = default);
}