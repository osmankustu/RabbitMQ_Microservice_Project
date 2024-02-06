using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using Polly;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.SeedWork;
using Microsoft.Data.SqlClient;
using Polly.Retry;

namespace OrderService.Infrastructure.Context
{
    public class OrderDbContextSeed
    {
        public async Task SeedAsync(OrderDbContext context,ILogger<OrderDbContext> logger)
        {
            var policy = CreatePolicy(logger, nameof(OrderDbContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = false;
                var contentRootPath = "Seeding/Setup";
                using (context)
                {
                    context.Database.Migrate();

                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(useCustomizationData
                                                   ? GetCardTypesFromFile(contentRootPath, logger)
                                                   : GetPredefineCardTypes());
                        await context.SaveChangesAsync();
                    }

                    if (!context.OrderStatus.Any())
                    {
                        context.OrderStatus.AddRange(useCustomizationData
                                                     ? GetOrderStatusFromFile(contentRootPath, logger)
                                                     : GetPredefineOrderStatus());
                    }

                    await context.SaveChangesAsync();
                }
            });
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<OrderDbContext> logger, string perifx,int retries=3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (excep, timespan, retry, ctx) =>
                {
                    logger.LogWarning(excep, "perfix");
                });

        }

        private IEnumerable<OrderStatus> GetPredefineOrderStatus()
        {
            return Enumeration.GetAll<OrderStatus>();
        }

        private IEnumerable<OrderStatus> GetOrderStatusFromFile(string contentRootPath, ILogger<OrderDbContext> logger)
        {
            string fileName = "OrderStatus.txt";

            if (!File.Exists(fileName))
            {
                return GetPredefineOrderStatus();
            }

            var fileContent = File.ReadAllLines(fileName);

            int id = 1;
            var list = fileContent.Select(i => new OrderStatus(id++, i)).Where(i => i != null);

            return list;

        }

        private IEnumerable<CardType> GetPredefineCardTypes()
        {
            return Enumeration.GetAll<CardType>();
        }

        private IEnumerable<CardType> GetCardTypesFromFile(string contentRootPath, ILogger<OrderDbContext> logger)
        {
            string fileName = "CardTypes.txt";

            if(!File.Exists(fileName))
            {
                return GetPredefineCardTypes();
            }

            var fileContent = File.ReadAllLines(fileName);

            int id = 1;
            var list = fileContent.Select(i => new CardType(id++, i)).Where(i=>i != null);

            return list;
        }
    }
}
