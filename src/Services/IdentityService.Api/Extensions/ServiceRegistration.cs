using IdentityService.Api.Application.Services;

namespace IdentityService.Api.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection serviceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IIdentityService, IdentityService.Api.Application.Services.IdentityService>();
            services.ConfigureConsul(configuration);

            return services;
        }
    }
}
