using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyCalendar.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CompanyCalendar.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILogger<ApplicationUser>>();

        await CreateRolesAsync(roleManager, logger);

        var seedOptions = configuration
            .GetSection(IdentitySeedOptions.SectionName)
            .Get<IdentitySeedOptions>();

        if (seedOptions is null)
        {
            logger.LogWarning(
                "IdentitySeed ayarları bulunamadığı için admin oluşturulmadı.");

            return;
        }

        await CreateAdminUserAsync(
            userManager,
            seedOptions,
            logger);
    }

    private static async Task CreateRolesAsync(
        RoleManager<IdentityRole> roleManager,
        ILogger logger)
    {
        foreach (var roleName in RoleNames.All)
        {
            var roleExists = await roleManager
                .RoleExistsAsync(roleName);

            if (roleExists)
            {
                continue;
            }

            var result = await roleManager.CreateAsync(
                new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"{roleName} rolü oluşturulamadı: {errors}");
            }

            logger.LogInformation(
                "{RoleName} rolü oluşturuldu.",
                roleName);
        }
    }

    private static async Task CreateAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IdentitySeedOptions options,
        ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(options.AdminEmail) ||
            string.IsNullOrWhiteSpace(options.AdminPassword))
        {
            logger.LogWarning(
                "Admin e-posta veya parola ayarı boş olduğu için admin oluşturulmadı.");

            return;
        }

        var normalizedEmail = options.AdminEmail.Trim();

        var adminUser = await userManager
            .FindByEmailAsync(normalizedEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                FirstName = options.AdminFirstName.Trim(),
                LastName = options.AdminLastName.Trim(),
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(
                adminUser,
                options.AdminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Admin kullanıcısı oluşturulamadı: {errors}");
            }

            logger.LogInformation(
                "İlk sistem yöneticisi oluşturuldu: {Email}",
                normalizedEmail);
        }

        var isSystemAdmin = await userManager.IsInRoleAsync(
            adminUser,
            RoleNames.SystemAdmin);

        if (!isSystemAdmin)
        {
            var roleResult = await userManager.AddToRoleAsync(
                adminUser,
                RoleNames.SystemAdmin);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Admin rolü kullanıcıya atanamadı: {errors}");
            }
        }
    }
}
