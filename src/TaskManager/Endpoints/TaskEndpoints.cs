using TaskManager.Services;

namespace TaskManager.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(WebApplication app)
    {
        app.MapPost("/api/tasks", PostTask);
        app.MapPost("/api/pause", PauseEndpoint);
        app.MapPost("/api/resume", ResumeEndpoint);
    }

    private static async Task<IResult> PostTask(string task, TaskQueueService queueService)
    {
        if (string.IsNullOrWhiteSpace(task))
            return Results.BadRequest("Task cannot be empty.");
        await queueService.EnqueueAsync(task);
        return Results.Accepted();
    }

    private static IResult PauseEndpoint(IPauseService pauseService)
    {
        pauseService.Pause();
        return Results.Ok("Paused");
    }

    private static IResult ResumeEndpoint(IPauseService pauseService)
    {
        pauseService.Resume();
        return Results.Ok("Resumed");
    }
}