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

public class PositionRepository : IPositionRepository
{
    private readonly ApplicationDbContext _context;

    public PositionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Position>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Positions
            .AsNoTracking()
            .Include(x => x.Department)
            .OrderBy(x => x.Department.Name)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Position?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Positions
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Position?> GetByIdWithDepartmentAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Positions
            .AsNoTracking()
            .Include(x => x.Department)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> NameExistsInDepartmentAsync(
        string name,
        int departmentId,
        int? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();

        return await _context.Positions.AnyAsync(
            x =>
                x.Name == normalizedName &&
                x.DepartmentId == departmentId &&
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

        return await _context.Positions.AnyAsync(
            x =>
                x.Code == normalizedCode &&
                (!excludedId.HasValue ||
                 x.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        Position position,
        CancellationToken cancellationToken = default)
    {
        await _context.Positions.AddAsync(
            position,
            cancellationToken);
    }

    public void Update(Position position)
    {
        _context.Positions.Update(position);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
