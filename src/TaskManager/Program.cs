using Scalar.AspNetCore;
using TaskManager.Options;
using TaskManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<QueueServiceOptions>(
    builder.Configuration.GetSection("Queue"));
builder.Services.Configure<BackgroundProcessorOptions>(
    builder.Configuration.GetSection("BackgroundProcessor"));

builder.Services.AddSingleton<PauseService>();
builder.Services.AddSingleton<QueueService>();
builder.Services.AddHostedService<BackgroundProcessor>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

TaskManager.Endpoints.TaskEndpoints.MapTaskEndpoints(app);

app.Run();
