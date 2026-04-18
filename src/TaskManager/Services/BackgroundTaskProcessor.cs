using Microsoft.Extensions.Options;
using TaskManager.Options;

namespace TaskManager.Services;

public partial class BackgroundTaskProcessor(
    IPauseService pauseService,
    TaskQueueService queueService,
    ILogger<BackgroundTaskProcessor> logger,
    IOptions<BackgroundTaskProcessorOptions> options)
    : BackgroundService
{
    private readonly int _workerCount = options.Value.WorkerCount;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();
        for (var i = 0; i < _workerCount; i++)
        {
            var workerId = i + 1;
            tasks.Add(Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await pauseService.WaitIfPaused(stoppingToken);

                    var task = await queueService.DequeueAsync(stoppingToken);
                    LogTask(DateTime.UtcNow, workerId, task);

                    //Mock
                    //await Task.Delay(1000, stoppingToken);
                }
            }, stoppingToken));
        }
        return Task.WhenAll(tasks);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Worker {workerId}: {timespan:HH:mm:ss} - {task}")]
    private partial void LogTask(DateTime timespan, int workerId, string task);
}