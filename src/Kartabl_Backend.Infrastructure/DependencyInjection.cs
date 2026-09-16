using System.Text;
using Kartabl_Backend.Application.Common.Interfaces;
using Kartabl_Backend.Infrastructure.Authentication;
using Kartabl_Backend.Infrastructure.Persistence;
using Kartabl_Backend.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Kartabl_Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // 1. Fetch Connection String
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // 2. Bind JwtOptions from appsettings.json
        var jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // 3. Register Authentication & Token Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // 4. Configure Database Context (SQL Server)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // 5. Register Database Initialiser
        services.AddScoped<ApplicationDbContextInitialiser>();

        // 6. Configure JWT Bearer Authentication & Token Validation
        var secretKey = jwtOptions.Secret ?? "DefaultFallbackSecretKeyForDevelopmentPhaseOnly32Bytes!";

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            });

        // 7. Register Repositories & UnitOfWork
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}