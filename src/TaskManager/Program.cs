using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TaskManager.Services.TaskQueueService>();
builder.Services.AddSingleton<TaskManager.Services.IPauseService, TaskManager.Services.PauseServiceTcs>();
builder.Services.Configure<TaskManager.Options.BackgroundTaskProcessorOptions>(
    builder.Configuration.GetSection("BackgroundTaskProcessor"));
builder.Services.AddHostedService<TaskManager.Services.BackgroundTaskProcessor>();

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
