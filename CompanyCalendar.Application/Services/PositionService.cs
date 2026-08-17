using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Application.DTOs.Positions;
using CompanyCalendar.Application.Interfaces.Repositories;
using CompanyCalendar.Application.Interfaces.Services;
using CompanyCalendar.Domain.Entities;

namespace CompanyCalendar.Application.Services;

public class PositionService : IPositionService
{
    private readonly IPositionRepository _positionRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public PositionService(
        IPositionRepository positionRepository,
        IDepartmentRepository departmentRepository)
    {
        _positionRepository = positionRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<List<PositionListDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var positions = await _positionRepository.GetAllAsync(
            cancellationToken);

        return positions.Select(x => new PositionListDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            DepartmentName = x.Department.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task<PositionDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository
            .GetByIdWithDepartmentAsync(
                id,
                cancellationToken);

        if (position is null)
        {
            return null;
        }

        return new PositionDetailDto
        {
            Id = position.Id,
            Name = position.Name,
            Code = position.Code,
            Description = position.Description,
            DepartmentId = position.DepartmentId,
            DepartmentName = position.Department.Name,
            IsActive = position.IsActive,
            CreatedAt = position.CreatedAt,
            UpdatedAt = position.UpdatedAt
        };
    }

    public async Task<UpdatePositionDto?> GetForUpdateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (position is null)
        {
            return null;
        }

        return new UpdatePositionDto
        {
            Id = position.Id,
            Name = position.Name,
            Code = position.Code,
            Description = position.Description,
            DepartmentId = position.DepartmentId,
            IsActive = position.IsActive
        };
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreatePositionDto dto,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(
            dto.DepartmentId,
            cancellationToken);

        if (department is null || !department.IsActive)
        {
            return (
                false,
                "Seçilen departman bulunamadı veya aktif değil.");
        }

        var name = dto.Name.Trim();
        var code = NormalizeNullable(dto.Code)?.ToUpperInvariant();

        var nameExists =
            await _positionRepository.NameExistsInDepartmentAsync(
                name,
                dto.DepartmentId,
                cancellationToken: cancellationToken);

        if (nameExists)
        {
            return (
                false,
                "Bu departmanda aynı isimde bir pozisyon zaten bulunmaktadır.");
        }

        if (code is not null)
        {
            var codeExists = await _positionRepository.CodeExistsAsync(
                code,
                cancellationToken: cancellationToken);

            if (codeExists)
            {
                return (
                    false,
                    "Bu pozisyon kodu sistemde zaten kullanılmaktadır.");
            }
        }

        var position = new Position
        {
            Name = name,
            Code = code,
            Description = NormalizeNullable(dto.Description),
            DepartmentId = dto.DepartmentId,
            IsActive = dto.IsActive,
            CreatedByUserId = userId
        };

        await _positionRepository.AddAsync(
            position,
            cancellationToken);

        await _positionRepository.SaveChangesAsync(
            cancellationToken);

        return (
            true,
            "Pozisyon başarıyla oluşturuldu.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        UpdatePositionDto dto,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository.GetByIdAsync(
            dto.Id,
            cancellationToken);

        if (position is null)
        {
            return (
                false,
                "Güncellenecek pozisyon bulunamadı.");
        }

        var department = await _departmentRepository.GetByIdAsync(
            dto.DepartmentId,
            cancellationToken);

        if (department is null || !department.IsActive)
        {
            return (
                false,
                "Seçilen departman bulunamadı veya aktif değil.");
        }

        var name = dto.Name.Trim();
        var code = NormalizeNullable(dto.Code)?.ToUpperInvariant();

        var nameExists =
            await _positionRepository.NameExistsInDepartmentAsync(
                name,
                dto.DepartmentId,
                dto.Id,
                cancellationToken);

        if (nameExists)
        {
            return (
                false,
                "Bu departmanda aynı isimde başka bir pozisyon bulunmaktadır.");
        }

        if (code is not null)
        {
            var codeExists =
                await _positionRepository.CodeExistsAsync(
                    code,
                    dto.Id,
                    cancellationToken);

            if (codeExists)
            {
                return (
                    false,
                    "Bu pozisyon kodu başka bir kayıtta kullanılmaktadır.");
            }
        }

        position.Name = name;
        position.Code = code;
        position.Description = NormalizeNullable(dto.Description);
        position.DepartmentId = dto.DepartmentId;
        position.IsActive = dto.IsActive;
        position.UpdatedByUserId = userId;

        _positionRepository.Update(position);

        await _positionRepository.SaveChangesAsync(
            cancellationToken);

        return (
            true,
            "Pozisyon bilgileri başarıyla güncellendi.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(
        int id,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (position is null)
        {
            return (
                false,
                "Silinecek pozisyon bulunamadı.");
        }

        position.IsDeleted = true;
        position.IsActive = false;
        position.DeletedAt = DateTime.UtcNow;
        position.DeletedByUserId = userId;
        position.UpdatedByUserId = userId;

        _positionRepository.Update(position);

        await _positionRepository.SaveChangesAsync(
            cancellationToken);

        return (
            true,
            "Pozisyon kaydı güvenli şekilde kaldırıldı.");
    }

    private static string? NormalizeNullable(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}