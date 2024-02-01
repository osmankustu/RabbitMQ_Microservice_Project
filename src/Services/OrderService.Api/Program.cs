using EventBus.Base;
using EventBus.Base.Abstract;
using EventBus.Factory;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
});

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
            HostName = "192.168.1.39",
            UserName = "root",
            Password = "root",
            Port = 5672,
            AuthMechanisms = new List<IAuthMechanismFactory>() { default },
            TopologyRecoveryExceptionHandler = default,
            Uri = new Uri("amqp://root:root@192.168.1.39:5672"),
        }

    };
    return EventBusFactory.Create(config, sp);
});

var app = builder.Build();

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
