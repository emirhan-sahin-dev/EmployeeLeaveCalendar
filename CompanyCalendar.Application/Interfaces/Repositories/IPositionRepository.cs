using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Entities;

namespace CompanyCalendar.Application.Interfaces.Repositories;

public interface IPositionRepository
{
    Task<List<Position>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Position?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Position?> GetByIdWithDepartmentAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsInDepartmentAsync(
        string name,
        int departmentId,
        int? excludedId = null,
        CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(
        string code,
        int? excludedId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Position position,
        CancellationToken cancellationToken = default);

    void Update(Position position);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}