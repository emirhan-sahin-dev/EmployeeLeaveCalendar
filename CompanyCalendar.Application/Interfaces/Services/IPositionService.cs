using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Application.DTOs.Positions;

namespace CompanyCalendar.Application.Interfaces.Services;

public interface IPositionService
{
    Task<List<PositionListDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<PositionDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<UpdatePositionDto?> GetForUpdateAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> CreateAsync(
        CreatePositionDto dto,
        string? userId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> UpdateAsync(
        UpdatePositionDto dto,
        string? userId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> DeleteAsync(
        int id,
        string? userId,
        CancellationToken cancellationToken = default);
}
