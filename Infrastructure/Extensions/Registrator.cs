using Config;
using Infrastructure.Contexts;
using Infrastructure.Services.FetchBinaryService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class Registrator
{
    
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration config, string connectionString)
    {
        services.RegisterConfiguration(config);
        services.RegisterDbContext(connectionString);
        services.AddHttpClient();
        services.RegisterApiServices();
        return services;
    }
    
    private static IServiceCollection RegisterDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TemplateContext>(options =>
            options.UseMySQL(connectionString)
        );

        return services;
    }
        
    private static IServiceCollection RegisterApiServices(this IServiceCollection services)
    {
        services.AddScoped<IFetchBinaryService, FetchBinaryService>();
        return services;
    }

    private static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ConnectionStringConfig>(config.GetSection("ConnectionStrings"));
        services.Configure<UpstreamConfig>(config.GetSection("UpstreamConfig"));
        return services;
    }
}