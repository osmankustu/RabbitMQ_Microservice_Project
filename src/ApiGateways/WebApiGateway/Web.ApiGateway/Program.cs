using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder();

//var conf =builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
//{
//    config
//        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
//        .AddJsonFile("Configurations/ocelot.json")
//        .AddEnvironmentVariables();
//});

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
        .AddJsonFile("Configurations/ocelot.json")
        .AddEnvironmentVariables();
    });


// Add services to the container.
builder.Services.AddOcelot().AddConsul();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOcelot();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
