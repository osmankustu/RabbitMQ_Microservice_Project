using StackExchange.Redis;

namespace BasketService.Api.Extensions
{
    public static class RedisRegistration
    {
        public static IServiceCollection ConfigureRedis(this IServiceCollection services,IConfiguration configuration)
        {
            var redisConfig = ConfigurationOptions.Parse($"{configuration["ConnectionStrings:Redis:Host"]}", true);
            redisConfig.ResolveDns = true;
            
            return services.AddSingleton(sp => ConnectionMultiplexer.Connect(redisConfig));
        }
    }
}
