using BasketService.Api.Core.Applicaton.Repository;
using BasketService.Api.Core.Applicaton.Services;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repository;
using BasketService.Api.IntegrationEvents.Event;
using BasketService.Api.IntegrationEvents.EventHandler;
using EventBus.Base;
using EventBus.Base.Abstract;
using EventBus.Factory;
using RabbitMQ.Client;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    var conf = new EventBusConfig()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "BasketService",
        EventBusType = EventBusType.RabbitMQ,
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

    return EventBusFactory.Create(conf, sp);
});

builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Subscribtion EventBus
var eventbus = app.Services.GetRequiredService<IEventBus>();
eventbus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

// Add middleware to the app.
app.RegisterWithConsul(app.Configuration,app.Lifetime);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
