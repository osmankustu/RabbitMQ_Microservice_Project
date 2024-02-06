using Polly;
using ProductService.Api.Extensions;
using ProductService.Api.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions());

// Add services to the container.
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Add middleware to the app.
app.RegisterWithConsul(app.Configuration,app.Lifetime);
app.MigrateDbContext<ProductContext>((context, services) =>
{
    var env = services.GetService<IWebHostEnvironment>();
    var logger = services.GetService<ILogger<ProductContext>>();
});


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
