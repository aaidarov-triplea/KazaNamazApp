using KN.Core.Interfaces;
using KN.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KN.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddknData(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(o => o.UseSqlServer(config.GetConnectionString("Default")));

        services.AddScoped<IKazaService, KazaService>();
        return services;
    }
}