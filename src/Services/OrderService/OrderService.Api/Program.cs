using EventBus.Base;
using EventBus.Base.Abstract;
using EventBus.Factory;
using Microsoft.AspNetCore.Connections;
using OrderService.Api.Extensions;
using OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using OrderService.Api.Extensions.Registration.ServiceDiscovery;
using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Context;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions());

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json",optional:false,reloadOnChange:true)
    .AddEnvironmentVariables();
});


// Add services to the container.
builder.Services.AddApplicationRegistration();
builder.Services.AddPersistanceRegistration(builder.Configuration);
builder.Services.ConfigureEventHandlers();
builder.Services.AddServiceDiscoveryRegistration(builder.Configuration);
builder.Services.AddSingleton<IEventBus>(sp =>
{   
    var config = new EventBusConfig()
    {
        ConnectionRetryCount = 5,
        SubscriberClientAppName = "OrderService",
        EventBusType = EventBusType.RabbitMQ,
        EventNameSuffix = "IntegrationEvent",
        Connection = new ConnectionFactory()
        {
            HostName = "192.168.1.3",
            UserName = "root",
            Password = "root",
            Port = 5672,
            AuthMechanisms = new List<IAuthMechanismFactory>() { default },
            TopologyRecoveryExceptionHandler = default,
            Uri = new Uri("amqp://root:root@192.168.1.3:5672"),
        }

    };
    return EventBusFactory.Create(config, sp);
});
builder.Services.AddLogging(configure =>
{
    configure.AddConsole();

});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Add middleware to the app.
app.RegisterWithConsul(app.Configuration, app.Lifetime);

app.MigrateDbContext<OrderDbContext>((context, services) =>
{
    var env = services.GetService<IHostEnvironment>();
    var logger = services.GetService<ILogger<OrderDbContext>>();

    var dbSeeder = new OrderDbContextSeed();
    dbSeeder.SeedAsync(context, logger).Wait();
   
});

var eventbus = app.Services.GetRequiredService<IEventBus>();
eventbus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
