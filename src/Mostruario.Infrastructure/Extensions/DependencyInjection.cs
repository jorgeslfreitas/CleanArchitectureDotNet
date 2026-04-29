using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mostruario.Application.Interfaces.Auth;
using Mostruario.Domain.Interfaces;
using Mostruario.Infrastructure.Configuration;
using Mostruario.Infrastructure.Data;
using Mostruario.Infrastructure.Repositories;
using Mostruario.Infrastructure.Security;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var jwtOptions = new JwtOptions
        {
            SecretKey = configuration["Jwt:SecretKey"] ?? string.Empty,
            Issuer = configuration["Jwt:Issuer"] ?? "MostruarioApi",
            Audience = configuration["Jwt:Audience"] ?? "MostruarioApi",
            ExpirationInMinutes = int.TryParse(
                configuration["Jwt:ExpirationInMinutes"],
                out var configuredExpiration)
                ? configuredExpiration
                : 60,
            RefreshTokenExpirationInDays = int.TryParse(
                configuration["Jwt:RefreshTokenExpirationInDays"],
                out var refreshDays)
                ? refreshDays
                : 7
        };

        jwtOptions.Validate();
        services.AddSingleton(jwtOptions);

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}
