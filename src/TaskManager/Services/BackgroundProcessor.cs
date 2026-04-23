using Microsoft.Extensions.Options;
using TaskManager.Options;

namespace TaskManager.Services;

public partial class BackgroundProcessor(
    IOptions<BackgroundProcessorOptions> options,
    ILogger<BackgroundProcessor> logger,
    QueueService queueService,
    PauseService pauseService) : BackgroundService
{
    private readonly int _workerCount = options.Value.WorkerCount;

    protected override Task ExecuteAsync(CancellationToken token) =>
        Task.WhenAll(Workers(token));

    private IEnumerable<Task> Workers(CancellationToken token)
    {
        for (var i = 0; i < _workerCount; i++)
        {
            var workerId = i + 1;
            yield return Task.Run(() => Work(workerId, token), token);
        }
    }

    private async Task Work(int workerId, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await pauseService.WaitIfPaused();
            var task = await queueService.DequeueAsync(token);
            LogTask(DateTime.UtcNow, workerId, task);
            await Task.Delay(task.Length * 100, token);
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Worker {workerId}: {timespan:HH:mm:ss} - {task}")]
    private partial void LogTask(DateTime timespan, int workerId, string task);
}