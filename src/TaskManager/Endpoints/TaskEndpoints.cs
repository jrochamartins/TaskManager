using Microsoft.AspNetCore.Mvc;
using TaskManager.Services;

namespace TaskManager.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(WebApplication app)
    {
        var group = app.MapGroup("api");
        group.MapPost("tasks", PostTask);
        group.MapPost("pause", PauseEndpoint);
        group.MapPost("resume", ResumeEndpoint);
    }

    private static async Task<IResult> PostTask([FromQuery] string? task, QueueService queueService)
    {
        if (string.IsNullOrWhiteSpace(task))
            return Results.BadRequest("Task cannot be empty.");

        await queueService.EnqueueAsync(task);
        return Results.Accepted();
    }

    private static IResult PauseEndpoint(PauseService pauseService)
    {
        pauseService.Pause();
        return Results.Ok("Paused");
    }

    private static IResult ResumeEndpoint(PauseService pauseService)
    {
        pauseService.Resume();
        return Results.Ok("Resumed");
    }
}