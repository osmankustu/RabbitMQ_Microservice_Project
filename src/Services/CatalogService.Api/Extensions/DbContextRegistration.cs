using Microsoft.EntityFrameworkCore;
using ProductService.Api.Infrastructure.Context;
using System.Reflection;

namespace ProductService.Api.Extensions
{
    public static class DbContextRegistration
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection service , IConfiguration configuration)
        {
            service.AddEntityFrameworkSqlServer()
                .AddDbContext<ProductContext>(opt =>
                {
                    opt.UseSqlServer(configuration["ConnectionString"], sqlServerOptionsAction: sqlOpt =>
                    {
                        sqlOpt.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                        sqlOpt.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
                });
            return service;
        }
    }
}
