using Infrastructure.Contexts;
using Infrastructure.Services.FetchBinaryService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class Registrator
{
    
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, string connectionString)
    {
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
}