using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

namespace OrderService.Api.Extensions.Registration.ServiceDiscovery
{
    public static class ConsulRegistration
    {
        public static IServiceCollection AddServiceDiscoveryRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));

            return services;
        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IConfiguration configuration, IHostApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            //Server IP

            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>();
            var uri = new Uri(configuration["ConsulConfig:AppServerAddress"]);
            var address = addresses.Addresses.FirstOrDefault();
            if (address != null)
            {
                uri = new Uri(address);
            };

            //Register Service

            var registration = new AgentServiceRegistration()
            {
                ID = $"OrderService",
                Name = "OrderService",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] { "Order Service", "Order", "JWT Required" }
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;

        }
    }
}
