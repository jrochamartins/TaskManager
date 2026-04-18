var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TaskManager.Services.TaskQueueService>();
builder.Services.AddSingleton<TaskManager.Services.PauseService>();
builder.Services.Configure<TaskManager.Options.BackgroundTaskProcessorOptions>(
    builder.Configuration.GetSection("BackgroundTaskProcessor"));
builder.Services.AddHostedService<TaskManager.Services.BackgroundTaskProcessor>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
    });
}

app.UseHttpsRedirection();

TaskManager.Endpoints.TaskEndpoints.MapTaskEndpoints(app);

app.Run();
