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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        List<Task> tasks = [];

        for (var i = 0; i < _workerCount; i++)
        {
            var workerId = i + 1;

            tasks.Add(Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await pauseService.WaitIfPaused();

                    var task = await queueService.DequeueAsync(stoppingToken);
                    LogTask(DateTime.UtcNow, workerId, task);

                    await Task.Delay(task.Length * 100, stoppingToken);
                }
            }, stoppingToken));

        }

        return Task.WhenAll(tasks);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Worker {workerId}: {timespan:HH:mm:ss} - {task}")]
    private partial void LogTask(DateTime timespan, int workerId, string task);
}