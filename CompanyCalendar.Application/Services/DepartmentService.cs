using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Application.DTOs.Departments;
using CompanyCalendar.Application.Interfaces.Repositories;
using CompanyCalendar.Application.Interfaces.Services;
using CompanyCalendar.Domain.Entities;

namespace CompanyCalendar.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(
        IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<List<DepartmentListDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository
            .GetAllAsync(cancellationToken);

        return departments.Select(x => new DepartmentListDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Description = x.Description,
            IsActive = x.IsActive,
            PositionCount = x.Positions.Count,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task<DepartmentDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository
            .GetByIdWithPositionsAsync(
                id,
                cancellationToken);

        if (department is null)
        {
            return null;
        }

        return new DepartmentDetailDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            Description = department.Description,
            IsActive = department.IsActive,
            PositionCount = department.Positions.Count,
            CreatedAt = department.CreatedAt,
            UpdatedAt = department.UpdatedAt
        };
    }

    public async Task<UpdateDepartmentDto?> GetForUpdateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository
            .GetByIdAsync(
                id,
                cancellationToken);

        if (department is null)
        {
            return null;
        }

        return new UpdateDepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            Description = department.Description,
            IsActive = department.IsActive
        };
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreateDepartmentDto dto,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var name = dto.Name.Trim();
        var code = NormalizeNullable(dto.Code)?.ToUpperInvariant();

        var nameExists = await _departmentRepository.NameExistsAsync(
            name,
            cancellationToken: cancellationToken);

        if (nameExists)
        {
            return (
                false,
                "Bu departman adı sistemde zaten kayıtlıdır.");
        }

        if (code is not null)
        {
            var codeExists = await _departmentRepository.CodeExistsAsync(
                code,
                cancellationToken: cancellationToken);

            if (codeExists)
            {
                return (
                    false,
                    "Bu departman kodu sistemde zaten kullanılmaktadır.");
            }
        }

        var department = new Department
        {
            Name = name,
            Code = code,
            Description = NormalizeNullable(dto.Description),
            IsActive = dto.IsActive,
            CreatedByUserId = userId
        };

        await _departmentRepository.AddAsync(
            department,
            cancellationToken);

        await _departmentRepository.SaveChangesAsync(
            cancellationToken);

        return (
            true,
            "Departman başarıyla oluşturuldu.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        UpdateDepartmentDto dto,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(
            dto.Id,
            cancellationToken);

        if (department is null)
        {
            return (
                false,
                "Güncellenecek departman bulunamadı.");
        }

        var name = dto.Name.Trim();
        var code = NormalizeNullable(dto.Code)?.ToUpperInvariant();

        var nameExists = await _departmentRepository.NameExistsAsync(
            name,
            dto.Id,
            cancellationToken);

        if (nameExists)
        {
            return (
                false,
                "Bu departman adı başka bir kayıtta kullanılmaktadır.");
        }

        if (code is not null)
        {
            var codeExists = await _departmentRepository.CodeExistsAsync(
                code,
                dto.Id,
                cancellationToken);

            if (codeExists)
            {
                return (
                    false,
                    "Bu departman kodu başka bir kayıtta kullanılmaktadır.");
            }
        }

        department.Name = name;
        department.Code = code;
        department.Description = NormalizeNullable(dto.Description);
        department.IsActive = dto.IsActive;
        department.UpdatedByUserId = userId;

        _departmentRepository.Update(department);

        await _departmentRepository.SaveChangesAsync(
            cancellationToken);

        return (
            true,
            "Departman bilgileri başarıyla güncellendi.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(
        int id,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository
            .GetByIdWithPositionsAsync(
                id,
                cancellationToken);

        if (department is null)
        {
            return (
                false,
                "Silinecek departman bulunamadı.");
        }

        if (department.Positions.Any())
        {
            return (
                false,
                "Bu departmana bağlı pozisyonlar bulunduğu için departman kaldırılamaz.");
        }

        department.IsDeleted = true;
        department.IsActive = false;
        department.DeletedAt = DateTime.UtcNow;
        department.DeletedByUserId = userId;
        department.UpdatedByUserId = userId;

        _departmentRepository.Update(department);

        await _departmentRepository.SaveChangesAsync(
            cancellationToken);

        return (
            true,
            "Departman kaydı güvenli şekilde kaldırıldı.");
    }

    private static string? NormalizeNullable(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}