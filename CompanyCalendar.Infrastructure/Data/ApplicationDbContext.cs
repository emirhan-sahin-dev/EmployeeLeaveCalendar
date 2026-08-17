using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Common;
using CompanyCalendar.Domain.Entities;
using CompanyCalendar.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CompanyCalendar.Infrastructure.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();

    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureDepartment(builder);
        ConfigurePosition(builder);
        ConfigureEmployee(builder);
        ConfigureLeaveType(builder);
        ConfigureLeaveRequest(builder);

        ApplySoftDeleteFilters(builder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
    private static void ConfigureLeaveRequest(ModelBuilder builder)
    {
        builder.Entity<LeaveRequest>(entity =>
        {
            entity.ToTable("LeaveRequests");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.Status)
                .IsRequired();

            entity.HasOne(x => x.Employee)
                .WithMany(x => x.LeaveRequests)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.LeaveType)
                .WithMany(x => x.LeaveRequests)
                .HasForeignKey(x => x.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
    private static void ConfigureEmployee(ModelBuilder builder)
    {
        builder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(150);

            entity.HasOne(x => x.Department)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");
        });
    }
    private static void ConfigureLeaveType(ModelBuilder builder)
    {
        builder.Entity<LeaveType>(entity =>
        {
            entity.ToTable("LeaveTypes");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Color)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });
    }
    private static void ConfigureDepartment(ModelBuilder builder)
    {
        builder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Code)
                .HasMaxLength(30);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.HasIndex(x => x.Code)
                .IsUnique()
                .HasFilter("[Code] IS NOT NULL");
        });
    }

    private static void ConfigurePosition(ModelBuilder builder)
    {
        builder.Entity<Position>(entity =>
        {
            entity.ToTable("Positions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Code)
                .HasMaxLength(30);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.HasOne(x => x.Department)
                .WithMany(x => x.Positions)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new
            {
                x.DepartmentId,
                x.Name
            }).IsUnique();

            entity.HasIndex(x => x.Code)
                .IsUnique()
                .HasFilter("[Code] IS NOT NULL");
        });
    }

    private static void ApplySoftDeleteFilters(
        ModelBuilder builder)
    {

        builder.Entity<Department>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Position>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Employee>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<LeaveType>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<LeaveRequest>()
            .HasQueryFilter(x => !x.IsDeleted);
    }
}
