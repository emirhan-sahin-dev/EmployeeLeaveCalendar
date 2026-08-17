using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Entities;

namespace CompanyCalendar.Application.Interfaces.Repositories;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Department?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Department?> GetByIdWithPositionsAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(
        string name,
        int? excludedId = null,
        CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(
        string code,
        int? excludedId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Department department,
        CancellationToken cancellationToken = default);

    void Update(Department department);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<List<Department>> GetActiveAsync(
    CancellationToken cancellationToken = default);
}
