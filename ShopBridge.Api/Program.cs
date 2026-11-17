using Serilog;
using ShopBridge.Api.ExceptionHandling;
using ShopBridge.Api.Middleware;
using ShopBridge.Application;
using ShopBridge.Infra;
using ShopBridge.Infra.Logging.Configuration;
using ShopBridge.Infra.Logging.Metadata;

var builder = WebApplication.CreateBuilder(args);

Serilog.Debugging.SelfLog.Enable(Console.Error);

builder.Host.UseSerilog((context, configuration) =>
{
    SerilogConfiguration.Configure(
        configuration,
        context.Configuration);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfraServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<LogMetadataInitializer>();

    await initializer.Initialize();
}

app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next(context);
});

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseMiddleware<LogMetadataMiddleware>(); 
app.UseMiddleware<RequestLoggingMiddleware>(); 
app.MapControllers();
app.Run();

public partial class Program
{
}