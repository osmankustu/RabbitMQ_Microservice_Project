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
builder.Services.EventBusServiceRegistration(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Add middleware to the app.
app.RegisterWithConsul(app.Configuration,app.Lifetime);

// Subscribtion EventBus middleware
var eventbus = app.Services.GetRequiredService<IEventBus>();
eventbus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();


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
