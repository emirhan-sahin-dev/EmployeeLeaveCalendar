using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Application.Interfaces.Repositories;
using CompanyCalendar.Domain.Entities;
using CompanyCalendar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanyCalendar.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public DepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Include(x => x.Positions)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Department?> GetByIdWithPositionsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Include(x => x.Positions)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        int? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();

        return await _context.Departments.AnyAsync(
            x =>
                x.Name == normalizedName &&
                (!excludedId.HasValue ||
                 x.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(
        string code,
        int? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim();

        return await _context.Departments.AnyAsync(
            x =>
                x.Code == normalizedCode &&
                (!excludedId.HasValue ||
                 x.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        Department department,
        CancellationToken cancellationToken = default)
    {
        await _context.Departments.AddAsync(
            department,
            cancellationToken);
    }

    public void Update(Department department)
    {
        _context.Departments.Update(department);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Department>> GetActiveAsync(
    CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
