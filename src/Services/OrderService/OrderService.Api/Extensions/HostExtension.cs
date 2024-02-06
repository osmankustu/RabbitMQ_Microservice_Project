﻿using Microsoft.EntityFrameworkCore;
using Polly;
using System.Data.SqlClient;


namespace OrderService.Api.Extensions
{
    public static class HostExtension
    {
        public static IHost MigrateDbContext<TContext>(this IHost host,Action<TContext,IServiceProvider> seeder) where TContext :DbContext
        {
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migration database associated with context {context}", typeof(TContext).Name);

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(8),

                        });

                    retry.Execute(()=> InvokeSeeder(seeder,context,services));

                    logger.LogInformation("Migrated Database");
                    
                }
                catch (Exception ex)
                {

                    logger.LogError("An error occured while migration the database used on context {DbContextName}",typeof(TContext).Name);
                }

                return host;
            }
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext? context, IServiceProvider services) where TContext : DbContext
        {
             context.Database.EnsureCreated();
             context.Database.Migrate();
             seeder(context, services);
        }
    }
}
